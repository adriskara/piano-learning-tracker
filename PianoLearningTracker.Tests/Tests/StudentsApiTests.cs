using System.Net;
using System.Net.Http.Json;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Tests.Tests;

public class StudentsApiTests
{
    private static Teacher SeedTeacher(PianoLearningTracker.DAL.PianoLearningTrackerDbContext db, int id = 500)
    {
        var teacher = new Teacher
        {
            Id = id,
            FirstName = "Test",
            LastName = "Teacher",
            Email = $"teacher{id}@test.com",
            PhoneNumber = "000-000-0000",
            YearsOfExperience = 5,
            HireDate = new DateTime(2020, 1, 1)
        };
        db.Teachers.Add(teacher);
        db.SaveChanges();
        return teacher;
    }

    // ── GET all ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        using var factory = new PianoWebApplicationFactory();
        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/students");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateClient().GetAsync("/api/students");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithQuery_ReturnsFilteredResults()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        SeedTeacher(db, 510);
        db.Students.Add(new Student
        {
            FirstName = "Zoran",
            LastName = "Testić",
            DateOfBirth = new DateTime(2010, 1, 1),
            Email = "zoran@test.com",
            PhoneNumber = "091-000-0000",
            EnrollmentDate = new DateTime(2022, 9, 1),
            Grade = 3,
            TeacherId = 510
        });
        db.SaveChanges();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/students?q=Zoran");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var students = await response.Content.ReadFromJsonAsync<List<StudentDTO>>();
        Assert.NotNull(students);
        Assert.Contains(students, s => s.FirstName == "Zoran");
    }

    // ── GET by id ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        SeedTeacher(db, 520);
        db.Students.Add(new Student
        {
            Id = 100,
            FirstName = "Luka",
            LastName = "Babić",
            DateOfBirth = new DateTime(2012, 4, 15),
            Email = "luka@test.com",
            PhoneNumber = "098-123-4567",
            EnrollmentDate = new DateTime(2022, 9, 1),
            Grade = 3,
            TeacherId = 520
        });
        db.SaveChanges();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/students/100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<StudentDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Luka", dto.FirstName);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/students/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── POST ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_CreatesAndReturns201_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        SeedTeacher(db, 530);

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            FirstName = "Petra",
            LastName = "Novak",
            DateOfBirth = "2010-11-03T00:00:00",
            Email = "petra@test.com",
            PhoneNumber = "099-234-5678",
            EnrollmentDate = "2022-09-01T00:00:00",
            Grade = 4,
            TeacherId = 530
        };

        var response = await client.PostAsJsonAsync("/api/students", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<StudentDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Petra", dto.FirstName);
    }

    [Fact]
    public async Task Post_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        var payload = new { FirstName = "X", LastName = "X", Email = "x@x.com", PhoneNumber = "000", EnrollmentDate = "2022-09-01T00:00:00", DateOfBirth = "2010-01-01T00:00:00", Grade = 1 };

        var response = await factory.CreateClient().PostAsJsonAsync("/api/students", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Returns400_WhenBodyIsMalformed()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var content = new StringContent("{ not valid json !!! }", System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/students", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── PUT ────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Put_UpdatesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        SeedTeacher(db, 540);
        db.Students.Add(new Student
        {
            Id = 200,
            FirstName = "Original",
            LastName = "Student",
            DateOfBirth = new DateTime(2012, 1, 1),
            Email = "original@test.com",
            PhoneNumber = "091-000-0001",
            EnrollmentDate = new DateTime(2022, 9, 1),
            Grade = 2,
            TeacherId = 540
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            FirstName = "Updated",
            LastName = "Student",
            DateOfBirth = "2012-01-01T00:00:00",
            Email = "updated@test.com",
            PhoneNumber = "091-000-0001",
            EnrollmentDate = "2022-09-01T00:00:00",
            Grade = 3,
            TeacherId = 540
        };

        var response = await client.PutAsJsonAsync("/api/students/200", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<StudentDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Updated", dto.FirstName);
    }

    [Fact]
    public async Task Put_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var payload = new { FirstName = "X", LastName = "X", Email = "x@x.com", PhoneNumber = "000", DateOfBirth = "2010-01-01T00:00:00", EnrollmentDate = "2022-09-01T00:00:00", Grade = 1 };
        var response = await client.PutAsJsonAsync("/api/students/99999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── DELETE ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_RemovesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        SeedTeacher(db, 550);
        db.Students.Add(new Student
        {
            Id = 300,
            FirstName = "ToDelete",
            LastName = "Student",
            DateOfBirth = new DateTime(2012, 1, 1),
            Email = "delete@test.com",
            PhoneNumber = "000-000-0000",
            EnrollmentDate = new DateTime(2022, 9, 1),
            Grade = 1,
            TeacherId = 550
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var response = await client.DeleteAsync("/api/students/300");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync("/api/students/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
