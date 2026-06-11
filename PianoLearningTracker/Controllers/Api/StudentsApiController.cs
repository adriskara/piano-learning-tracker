using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Controllers.Api
{
    [Route("api/students")]
    [ApiController]
    public class StudentsApiController : ControllerBase
    {
        private readonly PianoLearningTrackerDbContext _dbContext;

        public StudentsApiController(PianoLearningTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<StudentDTO>> Get([FromQuery] string? q = null)
        {
            var query = _dbContext.Students.Include(s => s.Teacher).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(s =>
                    s.FirstName.Contains(q) ||
                    s.LastName.Contains(q) ||
                    s.Email.Contains(q));
            }

            var students = query.ToList().Select(ToDTO).ToList();
            return Ok(students);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<StudentDTO> Get(int id)
        {
            var student = _dbContext.Students
                .Include(s => s.Teacher)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound();

            return Ok(ToDTO(student));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<StudentDTO> Post([FromBody] Student model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EnrollmentDate = model.EnrollmentDate,
                Grade = model.Grade,
                Notes = model.Notes,
                TeacherId = model.TeacherId
            };

            _dbContext.Students.Add(student);
            _dbContext.SaveChanges();

            var result = _dbContext.Students
                .Include(s => s.Teacher)
                .First(s => s.Id == student.Id);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, ToDTO(result));
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult<StudentDTO> Put(int id, [FromBody] Student model)
        {
            var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();

            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.DateOfBirth = model.DateOfBirth;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;
            student.EnrollmentDate = model.EnrollmentDate;
            student.Grade = model.Grade;
            student.Notes = model.Notes;
            student.TeacherId = model.TeacherId;

            _dbContext.SaveChanges();

            var result = _dbContext.Students
                .Include(s => s.Teacher)
                .First(s => s.Id == id);

            return Ok(ToDTO(result));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var student = _dbContext.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                return NotFound();

            _dbContext.Students.Remove(student);
            _dbContext.SaveChanges();
            return Ok();
        }

        private static StudentDTO ToDTO(Student s) => new StudentDTO
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            DateOfBirth = s.DateOfBirth,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            EnrollmentDate = s.EnrollmentDate,
            Grade = s.Grade,
            Notes = s.Notes,
            TeacherId = s.TeacherId,
            Teacher = s.Teacher == null ? null : new TeacherBriefDTO
            {
                Id = s.Teacher.Id,
                FirstName = s.Teacher.FirstName,
                LastName = s.Teacher.LastName,
                Email = s.Teacher.Email
            }
        };
    }
}
