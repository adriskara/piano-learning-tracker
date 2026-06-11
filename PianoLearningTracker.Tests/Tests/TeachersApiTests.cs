using System.Net;
using System.Net.Http.Json;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Tests.Tests;

public class TeachersApiTests
{
    // ── GET all ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        using var factory = new PianoWebApplicationFactory();
        var response = await factory.CreateClient().GetAsync("/api/teachers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithQuery_ReturnsFilteredResults()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Teachers.Add(new Teacher
        {
            FirstName = "UniqueFirstName",
            LastName = "Horvat",
            Email = "unique@test.com",
            PhoneNumber = "091-000-0001",
            YearsOfExperience = 5,
            HireDate = new DateTime(2020, 1, 1)
        });
        db.SaveChanges();

        var response = await factory.CreateClient().GetAsync("/api/teachers?q=UniqueFirstName");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var teachers = await response.Content.ReadFromJsonAsync<List<TeacherDTO>>();
        Assert.NotNull(teachers);
        Assert.Contains(teachers, t => t.FirstName == "UniqueFirstName");
    }

    // ── GET by id ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Teachers.Add(new Teacher
        {
            Id = 100,
            FirstName = "Marija",
            LastName = "Kovač",
            Email = "marija.kovac@test.com",
            PhoneNumber = "091-111-1111",
            YearsOfExperience = 10,
            HireDate = new DateTime(2015, 9, 1)
        });
        db.SaveChanges();

        var response = await factory.CreateClient().GetAsync("/api/teachers/100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<TeacherDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Marija", dto.FirstName);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateClient().GetAsync("/api/teachers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── POST ───────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Post_CreatesAndReturns201_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var payload = new
        {
            FirstName = "Ivan",
            LastName = "Babić",
            Email = "ivan.babic@test.com",
            PhoneNumber = "092-222-2222",
            Specialization = "Jazz",
            YearsOfExperience = 8,
            HireDate = "2019-01-01T00:00:00"
        };

        var response = await client.PostAsJsonAsync("/api/teachers", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<TeacherDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Ivan", dto.FirstName);
    }

    [Fact]
    public async Task Post_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        var payload = new { FirstName = "Test", LastName = "Test", Email = "t@t.com", PhoneNumber = "000", HireDate = "2020-01-01T00:00:00" };

        var response = await factory.CreateClient().PostAsJsonAsync("/api/teachers", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Returns400_WhenBodyIsMalformed()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var content = new StringContent("{ bad json }", System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/teachers", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── PUT ────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Put_UpdatesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Teachers.Add(new Teacher
        {
            Id = 200,
            FirstName = "Ana",
            LastName = "Jurić",
            Email = "ana@test.com",
            PhoneNumber = "095-333-3333",
            YearsOfExperience = 3,
            HireDate = new DateTime(2022, 1, 1)
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            FirstName = "Ana Updated",
            LastName = "Jurić",
            Email = "ana@test.com",
            PhoneNumber = "095-333-3333",
            YearsOfExperience = 4,
            HireDate = "2022-01-01T00:00:00"
        };

        var response = await client.PutAsJsonAsync("/api/teachers/200", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<TeacherDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Ana Updated", dto.FirstName);
    }

    [Fact]
    public async Task Put_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var payload = new { FirstName = "X", LastName = "X", Email = "x@x.com", PhoneNumber = "000", HireDate = "2020-01-01T00:00:00" };
        var response = await client.PutAsJsonAsync("/api/teachers/99999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── DELETE ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_RemovesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Teachers.Add(new Teacher
        {
            Id = 300,
            FirstName = "Delete",
            LastName = "Me",
            Email = "delete@test.com",
            PhoneNumber = "000-000-0000",
            YearsOfExperience = 1,
            HireDate = new DateTime(2024, 1, 1)
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var response = await client.DeleteAsync("/api/teachers/300");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync("/api/teachers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
