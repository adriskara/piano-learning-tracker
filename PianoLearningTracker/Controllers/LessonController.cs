using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    [Route("satovi")]
    public class LessonController : Controller
    {
        private readonly IMockRepository _repository;

        public LessonController(IMockRepository repository)
        {
            _repository = repository;
        }

        // URL: /satovi
        [Route("")]
        public IActionResult Index()
        {
            return View(_repository.GetAllLessons());
        }

        // Constraint :int osigurava da id mora biti cijeli broj (ne /satovi/detalji/abc)
        // URL: /satovi/detalji/1
        [Route("detalji/{id:int}")]
        public IActionResult Details(int id)
        {
            var lesson = _repository.GetLessonById(id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }
    }
}
