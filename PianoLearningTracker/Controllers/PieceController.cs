using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.DAL;
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
        private readonly IRepository _repository;
        private readonly PianoLearningTrackerDbContext _dbContext;

        public PieceController(IRepository repository, PianoLearningTrackerDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
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

        // Dropzone upload datoteke uz skladbu
        // URL: /skladbe/upload/1
        [Route("upload/{pieceId:int}")]
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult UploadAttachment(int pieceId, IFormFile file)
        {
            var piece = _repository.GetPieceById(pieceId);
            if (piece == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("Datoteka je prazna.");

            var uploadsPath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pieces", pieceId.ToString());
            Directory.CreateDirectory(uploadsPath);

            var safeFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, safeFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var attachment = new PieceAttachment
            {
                PieceId = pieceId,
                FileName = file.FileName,
                FilePath = "/uploads/pieces/" + pieceId + "/" + safeFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.PieceAttachments.Add(attachment);
            _dbContext.SaveChanges();

            return Json(new { success = true });
        }

        // AJAX dohvat popisa datoteka — vraća partial view
        // URL: /skladbe/datoteke/1
        [Route("datoteke/{pieceId:int}")]
        public IActionResult GetAttachments(int pieceId)
        {
            var attachments = _dbContext.PieceAttachments
                .Where(a => a.PieceId == pieceId)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
            return PartialView("_AttachmentList", attachments);
        }

        // AJAX brisanje datoteke
        // URL: /skladbe/obrisi-datoteku/1
        [Route("obrisi-datoteku/{id:int}")]
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteAttachment(int id)
        {
            var attachment = _dbContext.PieceAttachments.FirstOrDefault(a => a.Id == id);
            if (attachment == null)
                return NotFound();

            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot",
                attachment.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);

            _dbContext.PieceAttachments.Remove(attachment);
            _dbContext.SaveChanges();

            return Json(new { success = true });
        }
    }
}
