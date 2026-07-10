using System.Net;

namespace PianoLearningTracker.Tests.Tests;

// Provjera kontrole pristupa za /moj-profesor.
// Happy-path (Student vidi svog profesora) zahtijeva rola+user seeding infrastrukturu;
// ovdje se osigurava da endpoint nije dostupan neautoriziranim korisnicima.
public class MyTeacherTests
{
    [Fact]
    public async Task Get_Returns401_WhenNotAuthenticated()
    {
        using var factory = new PianoWebApplicationFactory();

        var response = await factory.CreateClient().GetAsync("/moj-profesor");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_Returns403_WhenAuthenticatedButNotStudent()
    {
        using var factory = new PianoWebApplicationFactory();

        // Autentificirani korisnik iz TestAuthHandler-a nema rolu "Student"
        var response = await factory.CreateAuthenticatedClient().GetAsync("/moj-profesor");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
