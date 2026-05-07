using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    // Semantički URL na hrvatskom; bazna ruta za sve akcije ovog controllera
    [Route("ucenici")]
    public class StudentController : Controller
    {
        private readonly IMockRepository _repository;

        public StudentController(IMockRepository repository)
        {
            _repository = repository;
        }

        // URL: /ucenici
        [Route("")]
        public IActionResult Index()
        {
            return View(_repository.GetAllStudents());
        }

        // Constraint :int osigurava da id mora biti cijeli broj (ne /ucenici/detalji/abc)
        // URL: /ucenici/detalji/1
        [Route("detalji/{id:int}")]
        public IActionResult Details(int id)
        {
            var student = _repository.GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
    }
}
