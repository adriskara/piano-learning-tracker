using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    public class PieceController : Controller
    {
        private readonly IMockRepository _repository;

        public PieceController(IMockRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAllPieces());
        }

        public IActionResult Details(int id)
        {
            var piece = _repository.GetPieceById(id);
            if (piece == null)
            {
                return NotFound();
            }

            return View(piece);
        }
    }
}
