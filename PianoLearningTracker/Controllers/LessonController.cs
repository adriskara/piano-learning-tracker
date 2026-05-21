using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    [Authorize]
    [Route("satovi")]
    public class LessonController : Controller
    {
        private readonly IMockRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LessonController(IMockRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // URL: /satovi
        [Route("")]
        public async Task<IActionResult> Index()
        {
            int? teacherId = null;
            int? studentId = null;
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                teacherId = user?.TeacherId;
            }
            else if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                studentId = user?.StudentId;
            }
            return View(_repository.SearchLessons("", teacherId, studentId));
        }

        // AJAX pretraga — vraća partial view s filtriranim satovima
        // URL: /satovi/pretraga?q=...
        [Route("pretraga")]
        public async Task<IActionResult> Search(string q = "")
        {
            int? teacherId = null;
            int? studentId = null;
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                teacherId = user?.TeacherId;
            }
            else if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                studentId = user?.StudentId;
            }
            return PartialView("_LessonList", _repository.SearchLessons(q, teacherId, studentId));
        }

        // Constraint :int osigurava da id mora biti cijeli broj (ne /satovi/detalji/abc)
        // URL: /satovi/detalji/1
        [Route("detalji/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var lesson = _repository.GetLessonById(id);
            if (lesson == null) return NotFound();

            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null && lesson.TeacherId != user.TeacherId)
                    return Forbid();
            }
            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.StudentId != null && lesson.StudentId != user.StudentId)
                    return Forbid();
            }

            return View(lesson);
        }

        // Prikaz forme za novi sat — Administrator i Teacher
        // URL: /satovi/novi
        [Route("novi")]
        [Authorize(Roles = "Administrator,Teacher")]
        public async Task<IActionResult> Create()
        {
            var model = new LessonFormModel
            {
                ScheduledDate = DateTime.Today.AddHours(16),
                DurationMinutes = 45,
                Status = LessonStatus.Scheduled
            };

            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null)
                {
                    var teacher = _repository.GetTeacherById(user.TeacherId.Value);
                    model.TeacherId = user.TeacherId.Value;
                    model.TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : null;
                }
            }

            return View(model);
        }

        [Route("novi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Teacher")]
        public async Task<IActionResult> Create(LessonFormModel model)
        {
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null)
                    model.TeacherId = user.TeacherId.Value;
            }

            if (!ModelState.IsValid)
                return View(model);

            var lesson = new Lesson
            {
                ScheduledDate = model.ScheduledDate,
                DurationMinutes = model.DurationMinutes,
                Status = model.Status,
                Notes = model.Notes,
                HomeworkAssigned = model.HomeworkAssigned,
                StudentId = model.StudentId,
                TeacherId = model.TeacherId
            };
            _repository.AddLesson(lesson);
            TempData["Success"] = "Sat je uspješno dodan.";
            return RedirectToAction(nameof(Index));
        }

        // Prikaz forme za uređivanje sata — ActionName jer oba imaju int id parametar
        // URL: /satovi/uredi/1
        [Route("uredi/{id:int}")]
        [Authorize(Roles = "Administrator,Teacher")]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(int id)
        {
            var lesson = _repository.GetLessonById(id);
            if (lesson == null) return NotFound();

            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null && lesson.TeacherId != user.TeacherId)
                    return Forbid();
            }

            return View(new LessonFormModel
            {
                ScheduledDate = lesson.ScheduledDate,
                DurationMinutes = lesson.DurationMinutes,
                Status = lesson.Status,
                Notes = lesson.Notes,
                HomeworkAssigned = lesson.HomeworkAssigned,
                StudentId = lesson.StudentId,
                StudentName = lesson.Student != null ? $"{lesson.Student.FirstName} {lesson.Student.LastName}" : null,
                TeacherId = lesson.TeacherId,
                TeacherName = lesson.Teacher != null ? $"{lesson.Teacher.FirstName} {lesson.Teacher.LastName}" : null
            });
        }

        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = "Administrator,Teacher")]
        public async Task<IActionResult> EditPost(int id, LessonFormModel model)
        {
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null)
                    model.TeacherId = user.TeacherId.Value;
            }

            if (!ModelState.IsValid)
                return View(model);

            var lesson = _repository.GetLessonById(id);
            if (lesson == null) return NotFound();

            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null && lesson.TeacherId != user.TeacherId)
                    return Forbid();
            }

            lesson.ScheduledDate = model.ScheduledDate;
            lesson.DurationMinutes = model.DurationMinutes;
            lesson.Status = model.Status;
            lesson.Notes = model.Notes;
            lesson.HomeworkAssigned = model.HomeworkAssigned;
            lesson.StudentId = model.StudentId;
            lesson.TeacherId = model.TeacherId;

            _repository.UpdateLesson(lesson);
            TempData["Success"] = "Sat je uspješno ažuriran.";
            return RedirectToAction("Details", new { id });
        }

        // Brisanje sata (POST zbog sigurnosti)
        // URL: /satovi/obrisi/1
        [Route("obrisi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var lesson = _repository.GetLessonById(id);
                if (lesson != null)
                {
                    if (User.IsInRole("Teacher"))
                    {
                        var user = await _userManager.GetUserAsync(User);
                        if (user?.TeacherId != null && lesson.TeacherId != user.TeacherId)
                            return Forbid();
                    }
                    _repository.DeleteLesson(id);
                    TempData["Success"] = "Sat je uspješno obrisan.";
                }
            }
            catch
            {
                TempData["Error"] = "Nije moguće obrisati sat.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
