using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    [Authorize(Roles = "Administrator,Teacher")]
    // Semantički URL na hrvatskom; bazna ruta za sve akcije ovog controllera
    [Route("ucenici")]
    public class StudentController : Controller
    {
        private readonly IMockRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(IMockRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // URL: /ucenici
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View(await GetStudentsForCurrentUser());
        }

        // AJAX pretraga — vraća partial view s filtriranim učenicima
        // URL: /ucenici/pretraga?q=...
        [Route("pretraga")]
        public async Task<IActionResult> Search(string q = "")
        {
            int? teacherId = null;
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                teacherId = user?.TeacherId;
            }
            return PartialView("_StudentList", _repository.SearchStudents(q, teacherId));
        }

        // Constraint :int osigurava da id mora biti cijeli broj (ne /ucenici/detalji/abc)
        // URL: /ucenici/detalji/1
        [Route("detalji/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var student = _repository.GetStudentById(id);
            if (student == null)
                return NotFound();

            // Teacher može vidjeti samo svoje učenike
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != student.TeacherId)
                    return Forbid();
            }

            return View(student);
        }

        // Prikaz forme za novog učenika — samo Administrator
        // URL: /ucenici/novi
        [Route("novi")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View(new StudentFormModel
            {
                EnrollmentDate = DateTime.Today,
                DateOfBirth = DateTime.Today.AddYears(-10)
            });
        }

        // Obrada forme za novog učenika (POST)
        // URL: /ucenici/novi
        [Route("novi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(StudentFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EnrollmentDate = model.EnrollmentDate,
                Grade = model.Grade,
                Notes = model.Notes,
                TeacherId = model.TeacherId
            };
            _repository.AddStudent(student);
            TempData["Success"] = $"Učenik {student.FirstName} {student.LastName} je uspješno dodan.";
            return RedirectToAction(nameof(Index));
        }

        // Prikaz forme za uređivanje učenika — ActionName jer oba imaju int id parametar
        // URL: /ucenici/uredi/1
        [Route("uredi/{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var student = _repository.GetStudentById(id);
            if (student == null)
                return NotFound();

            return View(new StudentFormModel
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                EnrollmentDate = student.EnrollmentDate,
                Grade = student.Grade,
                Notes = student.Notes,
                TeacherId = student.TeacherId,
                TeacherName = student.Teacher != null
                    ? $"{student.Teacher.FirstName} {student.Teacher.LastName}"
                    : null
            });
        }

        // Obrada forme za uređivanje učenika (POST)
        // URL: /ucenici/uredi/1
        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = "Administrator")]
        public IActionResult EditPost(int id, StudentFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = _repository.GetStudentById(id);
            if (student == null)
                return NotFound();

            // Mapiranje podataka forme na entitet (TryUpdateModel pristup ručno)
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.DateOfBirth = model.DateOfBirth;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;
            student.EnrollmentDate = model.EnrollmentDate;
            student.Grade = model.Grade;
            student.Notes = model.Notes;
            student.TeacherId = model.TeacherId;

            _repository.UpdateStudent(student);
            TempData["Success"] = $"Učenik {student.FirstName} {student.LastName} je uspješno ažuriran.";
            return RedirectToAction("Details", new { id });
        }

        // Brisanje učenika (POST zbog sigurnosti — GET ne bi trebao mijenjati stanje)
        // URL: /ucenici/obrisi/1
        [Route("obrisi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            try
            {
                var student = _repository.GetStudentById(id);
                if (student != null)
                {
                    var name = $"{student.FirstName} {student.LastName}";
                    _repository.DeleteStudent(id);
                    TempData["Success"] = $"Učenik {name} je uspješno obrisan.";
                }
            }
            catch
            {
                TempData["Error"] = "Nije moguće obrisati učenika jer ima povezane podatke.";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX autocomplete endpoint za dropdown — vraća JSON niz {id, text}
        // URL: /ucenici/autocomplete?q=...
        [Route("autocomplete")]
        public IActionResult Autocomplete(string q = "")
        {
            return Json(_repository.AutocompleteStudents(q));
        }

        // Pomoćna metoda — dohvaća učenike ovisno o ulozi prijavljenog korisnika
        private async Task<IReadOnlyList<Student>> GetStudentsForCurrentUser()
        {
            if (User.IsInRole("Teacher"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.TeacherId != null)
                    return _repository.GetStudentsByTeacherId(user.TeacherId.Value);
                return new List<Student>();
            }
            return _repository.GetAllStudents();
        }
    }
}
