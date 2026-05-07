using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    // Dvije bazne rute na controller razini — obje otvaraju isti controller
    [Route("skladbe")]
    [Route("repertoar")]
    public class PieceController : Controller
    {
        private readonly IMockRepository _repository;

        public PieceController(IMockRepository repository)
        {
            _repository = repository;
        }

        // URL: /skladbe  ILI  /repertoar
        [Route("")]
        public IActionResult Index()
        {
            return View(_repository.GetAllPieces());
        }

        // URL: /skladbe/detalji/1  ILI  /repertoar/detalji/1
        [Route("detalji/{id:int}")]
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
