using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IMockRepository _repository;

        public TeacherController(IMockRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAllTeachers());
        }

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
