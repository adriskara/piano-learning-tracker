using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Controllers.Api
{
    [Route("api/practice-sessions")]
    [ApiController]
    public class PracticeSessionsApiController : ControllerBase
    {
        private readonly PianoLearningTrackerDbContext _dbContext;

        public PracticeSessionsApiController(PianoLearningTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<PracticeSessionDTO>> Get([FromQuery] string? q = null)
        {
            var query = _dbContext.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(ps =>
                    ps.Student.FirstName.Contains(q) ||
                    ps.Student.LastName.Contains(q) ||
                    ps.Piece.Title.Contains(q) ||
                    (ps.Notes != null && ps.Notes.Contains(q)));
            }

            var sessions = query.ToList().Select(ToDTO).ToList();
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<PracticeSessionDTO> Get(int id)
        {
            var session = _dbContext.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .FirstOrDefault(ps => ps.Id == id);

            if (session == null)
                return NotFound();

            return Ok(ToDTO(session));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<PracticeSessionDTO> Post([FromBody] PracticeSession model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var session = new PracticeSession
            {
                Date = model.Date,
                DurationMinutes = model.DurationMinutes,
                Notes = model.Notes,
                QualityRating = model.QualityRating,
                Goals = model.Goals,
                StudentId = model.StudentId,
                PieceId = model.PieceId
            };

            _dbContext.PracticeSessions.Add(session);
            _dbContext.SaveChanges();

            var result = _dbContext.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .First(ps => ps.Id == session.Id);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, ToDTO(result));
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<PracticeSessionDTO> Put(int id, [FromBody] PracticeSession model)
        {
            var session = _dbContext.PracticeSessions.FirstOrDefault(ps => ps.Id == id);
            if (session == null)
                return NotFound();

            session.Date = model.Date;
            session.DurationMinutes = model.DurationMinutes;
            session.Notes = model.Notes;
            session.QualityRating = model.QualityRating;
            session.Goals = model.Goals;
            session.StudentId = model.StudentId;
            session.PieceId = model.PieceId;

            _dbContext.SaveChanges();

            var result = _dbContext.PracticeSessions
                .Include(ps => ps.Student)
                .Include(ps => ps.Piece)
                .First(ps => ps.Id == id);

            return Ok(ToDTO(result));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var session = _dbContext.PracticeSessions.FirstOrDefault(ps => ps.Id == id);
            if (session == null)
                return NotFound();

            _dbContext.PracticeSessions.Remove(session);
            _dbContext.SaveChanges();
            return Ok();
        }

        private static PracticeSessionDTO ToDTO(PracticeSession ps) => new PracticeSessionDTO
        {
            Id = ps.Id,
            Date = ps.Date,
            DurationMinutes = ps.DurationMinutes,
            Notes = ps.Notes,
            QualityRating = ps.QualityRating,
            Goals = ps.Goals,
            StudentId = ps.StudentId,
            Student = new StudentBriefDTO
            {
                Id = ps.Student.Id,
                FirstName = ps.Student.FirstName,
                LastName = ps.Student.LastName,
                Email = ps.Student.Email
            },
            PieceId = ps.PieceId,
            Piece = new PieceBriefDTO
            {
                Id = ps.Piece.Id,
                Title = ps.Piece.Title,
                Composer = ps.Piece.Composer,
                Difficulty = ps.Piece.Difficulty.ToString()
            }
        };
    }
}
