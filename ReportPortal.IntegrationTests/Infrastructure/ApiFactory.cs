using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReportPortal.DAL;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Boots the real API in-memory (TestServer) against a dedicated, disposable test database.
/// No migrations: the schema is created straight from the model (EnsureCreated), and rows are
/// wiped between tests for isolation.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    public const string TestConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=ReportPortal_IntegrationTests;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    // Child-before-parent order so the wipe respects foreign keys.
    private static readonly string[] TablesInDeleteOrder =
    {
        "UserSubprojects", "UserProjects", "TestReviews", "TestResults",
        "Tests", "Folders", "Runs", "Subprojects", "Projects", "Users"
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            // Repoint EF at the test database.
            services.RemoveAll<DbContextOptions<ApplicationContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<ApplicationContext>();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(TestConnectionString));
        });
    }

    /// <summary>Drops and recreates the schema from the model. Called once for the test assembly.</summary>
    public async Task CreateSchemaAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    /// <summary>Fast per-test isolation: wipe all rows (keeps the schema). Mini-Respawn.</summary>
    public async Task ResetAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var sql = string.Join(" ", TablesInDeleteOrder.Select(t => $"DELETE FROM [{t}];"));
        await db.Database.ExecuteSqlRawAsync(sql);
    }

    public async Task DropAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await db.Database.EnsureDeletedAsync();
    }
}
