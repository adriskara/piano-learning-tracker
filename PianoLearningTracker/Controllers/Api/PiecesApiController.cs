using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Controllers.Api
{
    [Route("api/pieces")]
    [ApiController]
    public class PiecesApiController : ControllerBase
    {
        private readonly PianoLearningTrackerDbContext _dbContext;

        public PiecesApiController(PianoLearningTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<PieceDTO>> Get([FromQuery] string? q = null)
        {
            var query = _dbContext.Pieces.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p =>
                    p.Title.Contains(q) ||
                    p.Composer.Contains(q) ||
                    (p.Genre != null && p.Genre.Contains(q)));
            }

            var pieces = query.ToList().Select(ToDTO).ToList();
            return Ok(pieces);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<PieceDTO> Get(int id)
        {
            var piece = _dbContext.Pieces.FirstOrDefault(p => p.Id == id);
            if (piece == null)
                return NotFound();

            return Ok(ToDTO(piece));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<PieceDTO> Post([FromBody] Piece model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

            _dbContext.Pieces.Add(piece);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = piece.Id }, ToDTO(piece));
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<PieceDTO> Put(int id, [FromBody] Piece model)
        {
            var piece = _dbContext.Pieces.FirstOrDefault(p => p.Id == id);
            if (piece == null)
                return NotFound();

            piece.Title = model.Title;
            piece.Composer = model.Composer;
            piece.Difficulty = model.Difficulty;
            piece.Genre = model.Genre;
            piece.DurationMinutes = model.DurationMinutes;
            piece.YearComposed = model.YearComposed;
            piece.Description = model.Description;

            _dbContext.SaveChanges();
            return Ok(ToDTO(piece));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var piece = _dbContext.Pieces.FirstOrDefault(p => p.Id == id);
            if (piece == null)
                return NotFound();

            _dbContext.Pieces.Remove(piece);
            _dbContext.SaveChanges();
            return Ok();
        }

        private static PieceDTO ToDTO(Piece p) => new PieceDTO
        {
            Id = p.Id,
            Title = p.Title,
            Composer = p.Composer,
            Difficulty = p.Difficulty.ToString(),
            Genre = p.Genre,
            DurationMinutes = p.DurationMinutes,
            YearComposed = p.YearComposed,
            Description = p.Description
        };
    }
}
