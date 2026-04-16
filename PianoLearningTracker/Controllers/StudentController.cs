using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    public class StudentController : Controller
    {
        private readonly IMockRepository _repository;

        public StudentController(IMockRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAllStudents());
        }

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
