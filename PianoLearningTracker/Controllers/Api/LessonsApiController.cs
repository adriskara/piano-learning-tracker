using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Controllers.Api
{
    [Route("api/lessons")]
    [ApiController]
    public class LessonsApiController : ControllerBase
    {
        private readonly PianoLearningTrackerDbContext _dbContext;

        public LessonsApiController(PianoLearningTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<LessonDTO>> Get([FromQuery] string? q = null)
        {
            var query = _dbContext.Lessons
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(l =>
                    l.Student.FirstName.Contains(q) ||
                    l.Student.LastName.Contains(q) ||
                    l.Teacher.FirstName.Contains(q) ||
                    l.Teacher.LastName.Contains(q) ||
                    (l.Notes != null && l.Notes.Contains(q)));
            }

            var lessons = query.ToList().Select(ToDTO).ToList();
            return Ok(lessons);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<LessonDTO> Get(int id)
        {
            var lesson = _dbContext.Lessons
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .FirstOrDefault(l => l.Id == id);

            if (lesson == null)
                return NotFound();

            return Ok(ToDTO(lesson));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<LessonDTO> Post([FromBody] Lesson model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lesson = new Lesson
            {
                ScheduledDate = model.ScheduledDate,
                DurationMinutes = model.DurationMinutes,
                Status = model.Status,
                Notes = model.Notes,
                HomeworkAssigned = model.HomeworkAssigned,
                StudentId = model.StudentId,
                TeacherId = model.TeacherId
            };

            _dbContext.Lessons.Add(lesson);
            _dbContext.SaveChanges();

            var result = _dbContext.Lessons
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .First(l => l.Id == lesson.Id);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, ToDTO(result));
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<LessonDTO> Put(int id, [FromBody] Lesson model)
        {
            var lesson = _dbContext.Lessons.FirstOrDefault(l => l.Id == id);
            if (lesson == null)
                return NotFound();

            lesson.ScheduledDate = model.ScheduledDate;
            lesson.DurationMinutes = model.DurationMinutes;
            lesson.Status = model.Status;
            lesson.Notes = model.Notes;
            lesson.HomeworkAssigned = model.HomeworkAssigned;
            lesson.StudentId = model.StudentId;
            lesson.TeacherId = model.TeacherId;

            _dbContext.SaveChanges();

            var result = _dbContext.Lessons
                .Include(l => l.Student)
                .Include(l => l.Teacher)
                .First(l => l.Id == id);

            return Ok(ToDTO(result));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var lesson = _dbContext.Lessons.FirstOrDefault(l => l.Id == id);
            if (lesson == null)
                return NotFound();

            _dbContext.Lessons.Remove(lesson);
            _dbContext.SaveChanges();
            return Ok();
        }

        private static LessonDTO ToDTO(Lesson l) => new LessonDTO
        {
            Id = l.Id,
            ScheduledDate = l.ScheduledDate,
            DurationMinutes = l.DurationMinutes,
            Status = l.Status.ToString(),
            Notes = l.Notes,
            HomeworkAssigned = l.HomeworkAssigned,
            StudentId = l.StudentId,
            Student = new StudentBriefDTO
            {
                Id = l.Student.Id,
                FirstName = l.Student.FirstName,
                LastName = l.Student.LastName,
                Email = l.Student.Email
            },
            TeacherId = l.TeacherId,
            Teacher = new TeacherBriefDTO
            {
                Id = l.Teacher.Id,
                FirstName = l.Teacher.FirstName,
                LastName = l.Teacher.LastName,
                Email = l.Teacher.Email
            }
        };
    }
}
