using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    public class PracticeSessionController : Controller
    {
        private readonly IMockRepository _repository;

        public PracticeSessionController(IMockRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAllPracticeSessions());
        }

        public IActionResult Details(int id)
        {
            var session = _repository.GetPracticeSessionById(id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }
    }
}
