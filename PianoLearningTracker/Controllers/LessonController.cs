using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    public class LessonController : Controller
    {
        private readonly IMockRepository _repository;

        public LessonController(IMockRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAllLessons());
        }

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
