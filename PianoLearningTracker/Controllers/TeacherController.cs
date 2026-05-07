using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
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

        // URL: /nastavnici/detalji/1  ILI  /profesori/detalji/1
        [Route("detalji/{id:int}")]
        public IActionResult Details(int id)
        {
            var teacher = _repository.GetTeacherById(id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
    }
}
