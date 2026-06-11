using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PianoLearningTracker.DAL;

namespace PianoLearningTracker.Tests;

public class PianoWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Google:ClientId"] = "test-client-id",
                ["Authentication:Google:ClientSecret"] = "test-client-secret",
            });
        });

        builder.ConfigureServices(services =>
        {
            // EF Core 8+ stores options callbacks as IDbContextOptionsConfiguration<T>.
            // Remove ALL existing ones for this context (e.g. the SqlServer one) so
            // only the InMemory configuration below gets applied.
            var optionsConfigType = typeof(IDbContextOptionsConfiguration<PianoLearningTrackerDbContext>);
            var toRemove = services
                .Where(d => d.ServiceType == optionsConfigType
                         || d.ServiceType == typeof(DbContextOptions<PianoLearningTrackerDbContext>))
                .ToList();
            foreach (var d in toRemove)
                services.Remove(d);

            services.AddDbContext<PianoLearningTrackerDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Override default auth scheme so [Authorize] uses our test handler
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultForbidScheme = TestAuthHandler.SchemeName;
            });

            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });
    }

    /// <summary>Returns a client that sends requests as an authenticated user.</summary>
    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", TestAuthHandler.AuthHeaderValue);
        return client;
    }

    /// <summary>Provides access to the DbContext for seeding test data.</summary>
    public PianoLearningTrackerDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<PianoLearningTrackerDbContext>();
    }
}
