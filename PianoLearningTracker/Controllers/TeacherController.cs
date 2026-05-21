using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    [Authorize]
    // Dvije rute na controller razini — obje otvaraju isti controller
    [Route("nastavnici")]
    [Route("profesori")]
    public class TeacherController : Controller
    {
        private readonly IMockRepository _repository;

        public TeacherController(IMockRepository repository)
        {
            _repository = repository;
        }

        // URL: /nastavnici  ILI  /profesori
        [Route("")]
        public IActionResult Index()
        {
            return View(_repository.GetAllTeachers());
        }

        // AJAX pretraga — vraća partial view s filtriranim profesorima
        // URL: /nastavnici/pretraga?q=...
        [Route("pretraga")]
        public IActionResult Search(string q = "")
        {
            return PartialView("_TeacherList", _repository.SearchTeachers(q));
        }

        // URL: /nastavnici/detalji/1  ILI  /profesori/detalji/1
        [Route("detalji/{id:int}")]
        public IActionResult Details(int id)
        {
            var teacher = _repository.GetTeacherById(id);
            if (teacher == null)
                return NotFound();
            return View(teacher);
        }

        // Prikaz forme za novog profesora — samo Administrator
        // URL: /nastavnici/novi
        [Route("novi")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View(new TeacherFormModel { HireDate = DateTime.Today });
        }

        // Obrada forme za novog profesora (POST)
        // URL: /nastavnici/novi
        [Route("novi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(TeacherFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var teacher = new Teacher
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Specialization = model.Specialization,
                YearsOfExperience = model.YearsOfExperience,
                HireDate = model.HireDate,
                Biography = model.Biography
            };
            _repository.AddTeacher(teacher);
            TempData["Success"] = $"Profesor {teacher.FirstName} {teacher.LastName} je uspješno dodan.";
            return RedirectToAction(nameof(Index));
        }

        // Prikaz forme za uređivanje profesora — ActionName jer oba imaju int id parametar
        // URL: /nastavnici/uredi/1
        [Route("uredi/{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var teacher = _repository.GetTeacherById(id);
            if (teacher == null)
                return NotFound();

            return View(new TeacherFormModel
            {
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                Specialization = teacher.Specialization,
                YearsOfExperience = teacher.YearsOfExperience,
                HireDate = teacher.HireDate,
                Biography = teacher.Biography
            });
        }

        // Obrada forme za uređivanje profesora (POST)
        // URL: /nastavnici/uredi/1
        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = "Administrator")]
        public IActionResult EditPost(int id, TeacherFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var teacher = _repository.GetTeacherById(id);
            if (teacher == null)
                return NotFound();

            teacher.FirstName = model.FirstName;
            teacher.LastName = model.LastName;
            teacher.Email = model.Email;
            teacher.PhoneNumber = model.PhoneNumber;
            teacher.Specialization = model.Specialization;
            teacher.YearsOfExperience = model.YearsOfExperience;
            teacher.HireDate = model.HireDate;
            teacher.Biography = model.Biography;

            _repository.UpdateTeacher(teacher);
            TempData["Success"] = $"Profesor {teacher.FirstName} {teacher.LastName} je uspješno ažuriran.";
            return RedirectToAction("Details", new { id });
        }

        // Brisanje profesora (POST zbog sigurnosti)
        // URL: /nastavnici/obrisi/1
        [Route("obrisi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            try
            {
                var teacher = _repository.GetTeacherById(id);
                if (teacher != null)
                {
                    var name = $"{teacher.FirstName} {teacher.LastName}";
                    _repository.DeleteTeacher(id);
                    TempData["Success"] = $"Profesor {name} je uspješno obrisan.";
                }
            }
            catch
            {
                TempData["Error"] = "Nije moguće obrisati profesora jer ima povezane podatke.";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX autocomplete endpoint za dropdown — vraća JSON niz {id, text}
        // URL: /nastavnici/autocomplete?q=...
        [Route("autocomplete")]
        public IActionResult Autocomplete(string q = "")
        {
            return Json(_repository.AutocompleteTeachers(q));
        }
    }
}
