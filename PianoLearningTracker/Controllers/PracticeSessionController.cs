using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    [Route("vjezbe")]
    public class PracticeSessionController : Controller
    {
        private readonly IMockRepository _repository;

        public PracticeSessionController(IMockRepository repository)
        {
            _repository = repository;
        }

        // URL: /vjezbe
        [Route("")]
        public IActionResult Index()
        {
            return View(_repository.GetAllPracticeSessions());
        }

        // URL: /vjezbe/detalji/1
        [Route("detalji/{id:int}")]
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
