using System.Net;
using System.Net.Http.Json;
using PianoLearningTracker.DAL;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Tests.Tests;

public class PracticeSessionsApiTests
{
    private static object StudentStub(Student s) => new
    {
        s.Id, s.FirstName, s.LastName, s.Email, s.PhoneNumber,
        s.DateOfBirth, s.EnrollmentDate, s.Grade
    };

    private static object PieceStub(Piece p) => new
    {
        p.Id, p.Title, p.Composer, p.Difficulty, p.DurationMinutes, p.YearComposed
    };

    private static (Student student, Piece piece) SeedStudentAndPiece(
        PianoLearningTrackerDbContext db, int studentId = 700, int pieceId = 701, int teacherId = 702)
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
        var piece = new Piece
        {
            Id = pieceId,
            Title = "Test Piece",
            Composer = "Test Composer",
            Difficulty = DifficultyLevel.Beginner,
            DurationMinutes = 3,
            YearComposed = 2000
        };
        db.Teachers.Add(teacher);
        db.Students.Add(student);
        db.Pieces.Add(piece);
        db.SaveChanges();
        return (student, piece);
    }

    // ── GET all ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        using var factory = new PianoWebApplicationFactory();
        var response = await factory.CreateClient().GetAsync("/api/practice-sessions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── GET by id ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, piece) = SeedStudentAndPiece(db, 710, 711, 712);

        db.PracticeSessions.Add(new PracticeSession
        {
            Id = 100,
            Date = new DateTime(2025, 3, 11, 17, 0, 0),
            DurationMinutes = 20,
            QualityRating = 4,
            Goals = "Improve left hand",
            StudentId = student.Id,
            PieceId = piece.Id
        });
        db.SaveChanges();

        var response = await factory.CreateClient().GetAsync("/api/practice-sessions/100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PracticeSessionDTO>();
        Assert.NotNull(dto);
        Assert.Equal(20, dto.DurationMinutes);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateClient().GetAsync("/api/practice-sessions/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── POST ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_CreatesAndReturns201_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, piece) = SeedStudentAndPiece(db, 720, 721, 722);

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            Date = "2025-04-01T17:00:00",
            DurationMinutes = 30,
            Notes = "Good session",
            QualityRating = 5,
            Goals = "Finish section A",
            StudentId = student.Id,
            PieceId = piece.Id,
            Student = StudentStub(student),
            Piece = PieceStub(piece)
        };

        var response = await client.PostAsJsonAsync("/api/practice-sessions", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PracticeSessionDTO>();
        Assert.NotNull(dto);
        Assert.Equal(30, dto.DurationMinutes);
        Assert.Equal(5, dto.QualityRating);
    }

    [Fact]
    public async Task Post_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        var payload = new { Date = "2025-04-01T17:00:00", DurationMinutes = 30, StudentId = 1, PieceId = 1 };

        var response = await factory.CreateClient().PostAsJsonAsync("/api/practice-sessions", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Returns400_WhenBodyIsMalformed()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var content = new StringContent("{ invalid json !!!", System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/practice-sessions", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── PUT ────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Put_UpdatesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, piece) = SeedStudentAndPiece(db, 730, 731, 732);

        db.PracticeSessions.Add(new PracticeSession
        {
            Id = 200,
            Date = new DateTime(2025, 3, 11, 17, 0, 0),
            DurationMinutes = 20,
            QualityRating = 3,
            StudentId = student.Id,
            PieceId = piece.Id
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            Date = "2025-03-11T17:00:00",
            DurationMinutes = 40,
            Notes = "Updated notes",
            QualityRating = 5,
            Goals = "Improved goals",
            StudentId = student.Id,
            PieceId = piece.Id,
            Student = StudentStub(student),
            Piece = PieceStub(piece)
        };

        var response = await client.PutAsJsonAsync("/api/practice-sessions/200", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PracticeSessionDTO>();
        Assert.NotNull(dto);
        Assert.Equal(40, dto.DurationMinutes);
        Assert.Equal(5, dto.QualityRating);
    }

    [Fact]
    public async Task Put_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var payload = new
        {
            Date = "2025-04-01T17:00:00",
            DurationMinutes = 30,
            QualityRating = 3,
            StudentId = 1,
            PieceId = 1,
            Student = new { Id = 1, FirstName = "Test", LastName = "T", Email = "t@t.com", PhoneNumber = "000", DateOfBirth = "2012-01-01T00:00:00", EnrollmentDate = "2022-01-01T00:00:00", Grade = 1 },
            Piece = new { Id = 1, Title = "Test", Composer = "Test", Difficulty = 0, DurationMinutes = 3, YearComposed = 2000 }
        };
        var response = await client.PutAsJsonAsync("/api/practice-sessions/99999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── DELETE ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_RemovesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        var (student, piece) = SeedStudentAndPiece(db, 740, 741, 742);

        db.PracticeSessions.Add(new PracticeSession
        {
            Id = 300,
            Date = new DateTime(2025, 5, 1, 10, 0, 0),
            DurationMinutes = 15,
            QualityRating = 4,
            StudentId = student.Id,
            PieceId = piece.Id
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var response = await client.DeleteAsync("/api/practice-sessions/300");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync("/api/practice-sessions/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
