using System.Net;
using System.Net.Http.Json;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Tests.Tests;

public class LessonsApiTests
{
    private static object TeacherStub(Teacher t) => new
    {
        t.Id, t.FirstName, t.LastName, t.Email, t.PhoneNumber,
        t.YearsOfExperience, t.HireDate
    };

    private static object StudentStub(Student s) => new
    {
        s.Id, s.FirstName, s.LastName, s.Email, s.PhoneNumber,
        s.DateOfBirth, s.EnrollmentDate, s.Grade
    };

    private static (Student student, Teacher teacher) SeedStudentAndTeacher(
        PianoLearningTrackerDbContext db, int studentId = 600, int teacherId = 601)
    {
        var teacher = new Teacher
        {
            Id = teacherId,
            FirstName = "Test",
            LastName = "Teacher",
            Email = $"teacher{teacherId}@test.com",
            PhoneNumber = "000-000-0000",
            YearsOfExperience = 5,
            HireDate = new DateTime(2020, 1, 1)
        };
        var student = new Student
        {
            Id = studentId,
            FirstName = "Test",
            LastName = "Student",
            DateOfBirth = new DateTime(2012, 1, 1),
            Email = $"student{studentId}@test.com",
            PhoneNumber = "000-000-0001",
            EnrollmentDate = new DateTime(2022, 9, 1),
            Grade = 3,
            TeacherId = teacherId
        };
        db.Teachers.Add(teacher);
        db.Students.Add(student);
        db.SaveChanges();
        return (student, teacher);
    }

    // ── GET all ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        using var factory = new PianoWebApplicationFactory();
        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/lessons");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateClient().GetAsync("/api/lessons");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── GET by id ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, teacher) = SeedStudentAndTeacher(db, 610, 611);

        db.Lessons.Add(new Lesson
        {
            Id = 100,
            ScheduledDate = new DateTime(2025, 3, 10, 16, 0, 0),
            DurationMinutes = 45,
            Status = LessonStatus.Scheduled,
            StudentId = student.Id,
            TeacherId = teacher.Id
        });
        db.SaveChanges();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/lessons/100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<LessonDTO>();
        Assert.NotNull(dto);
        Assert.Equal(45, dto.DurationMinutes);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/lessons/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── POST ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_CreatesAndReturns201_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, teacher) = SeedStudentAndTeacher(db, 620, 621);

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            ScheduledDate = "2025-04-01T16:00:00",
            DurationMinutes = 45,
            Status = "Scheduled",
            Notes = "Test lesson",
            HomeworkAssigned = "Practice scales",
            StudentId = student.Id,
            TeacherId = teacher.Id,
            Student = StudentStub(student),
            Teacher = TeacherStub(teacher)
        };

        var response = await client.PostAsJsonAsync("/api/lessons", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<LessonDTO>();
        Assert.NotNull(dto);
        Assert.Equal(45, dto.DurationMinutes);
    }

    [Fact]
    public async Task Post_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        var payload = new { ScheduledDate = "2025-04-01T16:00:00", DurationMinutes = 45, StudentId = 1, TeacherId = 1 };

        var response = await factory.CreateClient().PostAsJsonAsync("/api/lessons", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Returns400_WhenBodyIsMalformed()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var content = new StringContent("{ invalid }", System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/lessons", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── PUT ────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Put_UpdatesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, teacher) = SeedStudentAndTeacher(db, 630, 631);

        db.Lessons.Add(new Lesson
        {
            Id = 200,
            ScheduledDate = new DateTime(2025, 3, 10, 16, 0, 0),
            DurationMinutes = 30,
            Status = LessonStatus.Scheduled,
            StudentId = student.Id,
            TeacherId = teacher.Id
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            ScheduledDate = "2025-03-10T16:00:00",
            DurationMinutes = 60,
            Status = "Scheduled",
            Notes = "Updated notes",
            HomeworkAssigned = "Updated homework",
            StudentId = student.Id,
            TeacherId = teacher.Id,
            Student = StudentStub(student),
            Teacher = TeacherStub(teacher)
        };

        var response = await client.PutAsJsonAsync("/api/lessons/200", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<LessonDTO>();
        Assert.NotNull(dto);
        Assert.Equal(60, dto.DurationMinutes);
    }

    [Fact]
    public async Task Put_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var payload = new
        {
            ScheduledDate = "2025-04-01T16:00:00",
            DurationMinutes = 45,
            Status = "Scheduled",
            StudentId = 1,
            TeacherId = 1,
            Student = new { Id = 1, FirstName = "Test", LastName = "T", Email = "t@t.com", PhoneNumber = "000", DateOfBirth = "2012-01-01T00:00:00", EnrollmentDate = "2022-01-01T00:00:00", Grade = 1 },
            Teacher = new { Id = 1, FirstName = "Test", LastName = "T", Email = "t@t.com", PhoneNumber = "000", YearsOfExperience = 1, HireDate = "2020-01-01T00:00:00" }
        };
        var response = await client.PutAsJsonAsync("/api/lessons/99999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── DELETE ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_RemovesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, teacher) = SeedStudentAndTeacher(db, 640, 641);

        db.Lessons.Add(new Lesson
        {
            Id = 300,
            ScheduledDate = new DateTime(2025, 5, 1, 10, 0, 0),
            DurationMinutes = 45,
            Status = LessonStatus.Scheduled,
            StudentId = student.Id,
            TeacherId = teacher.Id
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var response = await client.DeleteAsync("/api/lessons/300");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync("/api/lessons/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
