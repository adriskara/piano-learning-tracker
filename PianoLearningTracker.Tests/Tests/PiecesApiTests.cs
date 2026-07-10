using System.Net;
using System.Net.Http.Json;
using PianoLearningTracker.Models;
using PianoLearningTracker.Models.DTOs;

namespace PianoLearningTracker.Tests.Tests;

public class PiecesApiTests
{
    // ── GET all ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/pieces");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateClient().GetAsync("/api/pieces");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithQuery_ReturnsFilteredResults()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Pieces.Add(new Piece
        {
            Title = "SearchablePiece",
            Composer = "TestComposer",
            Difficulty = DifficultyLevel.Beginner,
            DurationMinutes = 3,
            YearComposed = 2000
        });
        db.SaveChanges();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/pieces?q=SearchablePiece");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pieces = await response.Content.ReadFromJsonAsync<List<PieceDTO>>();
        Assert.NotNull(pieces);
        Assert.Contains(pieces, p => p.Title == "SearchablePiece");
    }

    // ── GET by id ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReturnsOk_WhenExists()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Pieces.Add(new Piece
        {
            Id = 100,
            Title = "Für Elise",
            Composer = "Beethoven",
            Difficulty = DifficultyLevel.Elementary,
            DurationMinutes = 3,
            YearComposed = 1810
        });
        db.SaveChanges();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/pieces/100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PieceDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Für Elise", dto.Title);
    }

    [Fact]
    public async Task GetById_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateAuthenticatedClient().GetAsync("/api/pieces/99999");

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
            Title = "New Sonata",
            Composer = "Mozart",
            Difficulty = 2,          // DifficultyLevel.Intermediate
            DurationMinutes = 10,
            YearComposed = 1780,
            Genre = "Klasika"
        };

        var response = await client.PostAsJsonAsync("/api/pieces", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PieceDTO>();
        Assert.NotNull(dto);
        Assert.Equal("New Sonata", dto.Title);
    }

    [Fact]
    public async Task Post_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateClient();

        var payload = new { Title = "Test", Composer = "Test", DurationMinutes = 5, YearComposed = 2000 };
        var response = await client.PostAsJsonAsync("/api/pieces", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_Returns400_WhenBodyIsMalformed()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var content = new StringContent("{ invalid json }", System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/pieces", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ── PUT ────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Put_UpdatesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Pieces.Add(new Piece
        {
            Id = 200,
            Title = "Original Title",
            Composer = "Bach",
            Difficulty = DifficultyLevel.Beginner,
            DurationMinutes = 2,
            YearComposed = 1720
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var payload = new
        {
            Title = "Updated Title",
            Composer = "Bach",
            Difficulty = "Beginner",
            DurationMinutes = 4,
            YearComposed = 1720
        };

        var response = await client.PutAsJsonAsync("/api/pieces/200", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PieceDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Updated Title", dto.Title);
    }

    [Fact]
    public async Task Put_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var payload = new { Title = "X", Composer = "X", DurationMinutes = 1, YearComposed = 2000 };
        var response = await client.PutAsJsonAsync("/api/pieces/99999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── DELETE ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_RemovesAndReturnsOk_WhenAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();
        using var db = factory.GetDbContext();
        db.Pieces.Add(new Piece
        {
            Id = 300,
            Title = "To Delete",
            Composer = "Test",
            Difficulty = DifficultyLevel.Beginner,
            DurationMinutes = 1,
            YearComposed = 2000
        });
        db.SaveChanges();

        var client = factory.CreateAuthenticatedClient();
        var response = await client.DeleteAsync("/api/pieces/300");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenNotFound()
    {
        using var factory = new PianoWebApplicationFactory();
        var client = factory.CreateAuthenticatedClient();

        var response = await client.DeleteAsync("/api/pieces/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
