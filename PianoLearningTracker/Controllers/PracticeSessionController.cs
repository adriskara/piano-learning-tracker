using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    [Authorize]
    [Route("vjezbe")]
    public class PracticeSessionController : Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PracticeSessionController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // URL: /vjezbe
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View(await GetSessionsForCurrentUser());
        }

        // AJAX pretraga — vraća partial view s filtriranim vježbama
        // URL: /vjezbe/pretraga?q=...
        [Route("pretraga")]
        public async Task<IActionResult> Search(string q = "")
        {
            int? studentId = null;
            int? teacherId = null;
            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                studentId = user?.StudentId;
            }
            else if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                teacherId = user?.TeacherId;
            }
            return PartialView("_PracticeSessionList", _repository.SearchPracticeSessions(q, studentId, teacherId));
        }

        // URL: /vjezbe/detalji/1
        [Route("detalji/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var session = _repository.GetPracticeSessionById(id);
            if (session == null)
                return NotFound();

            // Teacher vidi samo vježbe svojih učenika
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null && session.Student?.TeacherId != user.TeacherId)
                    return Forbid();
            }

            // Student vidi samo svoje vježbe
            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.StudentId != null && session.StudentId != user.StudentId)
                    return Forbid();
            }

            return View(session);
        }

        // Prikaz forme za novu vježbu
        // URL: /vjezbe/nova
        [Route("nova")]
        [Authorize(Roles = "Administrator,Teacher,Student")]
        public async Task<IActionResult> Create()
        {
            var model = new PracticeSessionFormModel
            {
                Date = DateTime.Now,
                DurationMinutes = 30,
                QualityRating = 3
            };

            // Student automatski postaje StudentId — ne može birati tuđe vježbe
            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.StudentId != null)
                {
                    var student = _repository.GetStudentById(user.StudentId.Value);
                    model.StudentId = user.StudentId.Value;
                    model.StudentName = student != null ? $"{student.FirstName} {student.LastName}" : null;
                }
            }

            return View(model);
        }

        // Obrada forme za novu vježbu (POST)
        // URL: /vjezbe/nova
        [Route("nova")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Teacher,Student")]
        public async Task<IActionResult> Create(PracticeSessionFormModel model)
        {
            // Student ne može mijenjati StudentId
            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.StudentId != null)
                    model.StudentId = user.StudentId.Value;
            }

            if (!ModelState.IsValid)
                return View(model);

            var session = new PracticeSession
            {
                Date = model.Date,
                DurationMinutes = model.DurationMinutes,
                Notes = model.Notes,
                QualityRating = model.QualityRating,
                Goals = model.Goals,
                StudentId = model.StudentId,
                PieceId = model.PieceId
            };
            _repository.AddPracticeSession(session);
            TempData["Success"] = "Vježba je uspješno dodana.";
            return RedirectToAction(nameof(Index));
        }

        // Prikaz forme za uređivanje vježbe — ActionName jer oba imaju int id parametar
        // URL: /vjezbe/uredi/1
        [Route("uredi/{id:int}")]
        [Authorize(Roles = "Administrator,Teacher,Student")]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(int id)
        {
            var session = _repository.GetPracticeSessionById(id);
            if (session == null)
                return NotFound();

            if (!await CanAccessSession(session))
                return Forbid();

            return View(new PracticeSessionFormModel
            {
                Date = session.Date,
                DurationMinutes = session.DurationMinutes,
                Notes = session.Notes,
                QualityRating = session.QualityRating,
                Goals = session.Goals,
                StudentId = session.StudentId,
                StudentName = session.Student != null ? $"{session.Student.FirstName} {session.Student.LastName}" : null,
                PieceId = session.PieceId,
                PieceName = session.Piece?.Title
            });
        }

        // Obrada forme za uređivanje vježbe (POST)
        // URL: /vjezbe/uredi/1
        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = "Administrator,Teacher,Student")]
        public async Task<IActionResult> EditPost(int id, PracticeSessionFormModel model)
        {
            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.StudentId != null)
                    model.StudentId = user.StudentId.Value;
            }

            if (!ModelState.IsValid)
                return View(model);

            var session = _repository.GetPracticeSessionById(id);
            if (session == null)
                return NotFound();

            if (!await CanAccessSession(session))
                return Forbid();

            session.Date = model.Date;
            session.DurationMinutes = model.DurationMinutes;
            session.Notes = model.Notes;
            session.QualityRating = model.QualityRating;
            session.Goals = model.Goals;
            session.StudentId = model.StudentId;
            session.PieceId = model.PieceId;

            _repository.UpdatePracticeSession(session);
            TempData["Success"] = "Vježba je uspješno ažurirana.";
            return RedirectToAction("Details", new { id });
        }

        // Brisanje vježbe (POST zbog sigurnosti)
        // URL: /vjezbe/obrisi/1
        [Route("obrisi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Teacher,Student")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var session = _repository.GetPracticeSessionById(id);
                if (session != null)
                {
                    if (!await CanAccessSession(session))
                        return Forbid();

                    _repository.DeletePracticeSession(id);
                    TempData["Success"] = "Vježba je uspješno obrisana.";
                }
            }
            catch
            {
                TempData["Error"] = "Nije moguće obrisati vježbu.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Provjera ima li prijavljeni korisnik pravo pristupa vježbi
        private async Task<bool> CanAccessSession(PracticeSession session)
        {
            if (User.IsInRole("Administrator")) return true;

            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                return user?.TeacherId == null || session.Student?.TeacherId == user.TeacherId;
            }

            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                return user?.StudentId == null || session.StudentId == user.StudentId;
            }

            return false;
        }

        // Pomoćna metoda — dohvaća vježbe ovisno o ulozi prijavljenog korisnika
        private async Task<IReadOnlyList<PracticeSession>> GetSessionsForCurrentUser()
        {
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null)
                    return _repository.GetPracticeSessionsByTeacherId(user.TeacherId.Value);
                return new List<PracticeSession>();
            }
            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.StudentId != null)
                    return _repository.GetPracticeSessionsByStudentId(user.StudentId.Value);
                return new List<PracticeSession>();
            }
            return _repository.GetAllPracticeSessions();
        }
    }
}
