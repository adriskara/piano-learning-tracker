using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Controllers.Api
{
    [Route("api/teachers")]
    [ApiController]
    [Authorize]
    public class TeachersApiController : ControllerBase
    {
        private readonly PianoLearningTrackerDbContext _dbContext;

        public TeachersApiController(PianoLearningTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TeacherDTO>> Get([FromQuery] string? q = null)
        {
            var query = _dbContext.Teachers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(t =>
                    t.FirstName.Contains(q) ||
                    t.LastName.Contains(q) ||
                    (t.Specialization != null && t.Specialization.Contains(q)));
            }

            var teachers = query.ToList().Select(ToDTO).ToList();
            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public ActionResult<TeacherDTO> Get(int id)
        {
            var teacher = _dbContext.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
                return NotFound();

            return Ok(ToDTO(teacher));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<TeacherDTO> Post([FromBody] Teacher model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var teacher = new Teacher
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Specialization = model.Specialization,
                YearsOfExperience = model.YearsOfExperience,
                HireDate = model.HireDate,
                Biography = model.Biography
            };

            _dbContext.Teachers.Add(teacher);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = teacher.Id }, ToDTO(teacher));
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<TeacherDTO> Put(int id, [FromBody] Teacher model)
        {
            var teacher = _dbContext.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
                return NotFound();

            teacher.FirstName = model.FirstName;
            teacher.LastName = model.LastName;
            teacher.Email = model.Email;
            teacher.PhoneNumber = model.PhoneNumber;
            teacher.Specialization = model.Specialization;
            teacher.YearsOfExperience = model.YearsOfExperience;
            teacher.HireDate = model.HireDate;
            teacher.Biography = model.Biography;

            _dbContext.SaveChanges();
            return Ok(ToDTO(teacher));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var teacher = _dbContext.Teachers.FirstOrDefault(t => t.Id == id);
            if (teacher == null)
                return NotFound();

            _dbContext.Teachers.Remove(teacher);
            _dbContext.SaveChanges();
            return Ok();
        }

        private static TeacherDTO ToDTO(Teacher t) => new TeacherDTO
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            PhoneNumber = t.PhoneNumber,
            Specialization = t.Specialization,
            YearsOfExperience = t.YearsOfExperience,
            HireDate = t.HireDate,
            Biography = t.Biography
        };
    }
}
