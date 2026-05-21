using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Models;
using PianoLearningTracker.Repositories;

namespace PianoLearningTracker.Controllers
{
    [Authorize]
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

        // AJAX pretraga — vraća partial view s filtriranim skladbama
        // URL: /skladbe/pretraga?q=...
        [Route("pretraga")]
        public IActionResult Search(string q = "")
        {
            return PartialView("_PieceList", _repository.SearchPieces(q));
        }

        // URL: /skladbe/detalji/1  ILI  /repertoar/detalji/1
        [Route("detalji/{id:int}")]
        public IActionResult Details(int id)
        {
            var piece = _repository.GetPieceById(id);
            if (piece == null)
                return NotFound();
            return View(piece);
        }

        // Prikaz forme za novu skladbu — samo Administrator
        // URL: /skladbe/nova
        [Route("nova")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View(new PieceFormModel { YearComposed = DateTime.Today.Year });
        }

        // Obrada forme za novu skladbu (POST)
        // URL: /skladbe/nova
        [Route("nova")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create(PieceFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var piece = new Piece
            {
                Title = model.Title,
                Composer = model.Composer,
                Difficulty = model.Difficulty,
                Genre = model.Genre,
                DurationMinutes = model.DurationMinutes,
                YearComposed = model.YearComposed,
                Description = model.Description
            };
            _repository.AddPiece(piece);
            TempData["Success"] = $"Skladba \"{piece.Title}\" je uspješno dodana.";
            return RedirectToAction(nameof(Index));
        }

        // Prikaz forme za uređivanje skladbe — ActionName jer oba imaju int id parametar
        // URL: /skladbe/uredi/1
        [Route("uredi/{id:int}")]
        [Authorize(Roles = "Administrator")]
        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var piece = _repository.GetPieceById(id);
            if (piece == null)
                return NotFound();

            return View(new PieceFormModel
            {
                Title = piece.Title,
                Composer = piece.Composer,
                Difficulty = piece.Difficulty,
                Genre = piece.Genre,
                DurationMinutes = piece.DurationMinutes,
                YearComposed = piece.YearComposed,
                Description = piece.Description
            });
        }

        // Obrada forme za uređivanje skladbe (POST)
        // URL: /skladbe/uredi/1
        [Route("uredi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        [Authorize(Roles = "Administrator")]
        public IActionResult EditPost(int id, PieceFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var piece = _repository.GetPieceById(id);
            if (piece == null)
                return NotFound();

            piece.Title = model.Title;
            piece.Composer = model.Composer;
            piece.Difficulty = model.Difficulty;
            piece.Genre = model.Genre;
            piece.DurationMinutes = model.DurationMinutes;
            piece.YearComposed = model.YearComposed;
            piece.Description = model.Description;

            _repository.UpdatePiece(piece);
            TempData["Success"] = $"Skladba \"{piece.Title}\" je uspješno ažurirana.";
            return RedirectToAction("Details", new { id });
        }

        // Brisanje skladbe (POST zbog sigurnosti)
        // URL: /skladbe/obrisi/1
        [Route("obrisi/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            try
            {
                var piece = _repository.GetPieceById(id);
                if (piece != null)
                {
                    var title = piece.Title;
                    _repository.DeletePiece(id);
                    TempData["Success"] = $"Skladba \"{title}\" je uspješno obrisana.";
                }
            }
            catch
            {
                TempData["Error"] = "Nije moguće obrisati skladbu jer ima povezane podatke.";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX autocomplete endpoint za dropdown — vraća JSON niz {id, text}
        // URL: /skladbe/autocomplete?q=...
        [Route("autocomplete")]
        public IActionResult Autocomplete(string q = "")
        {
            return Json(_repository.AutocompletePieces(q));
        }
    }
}
