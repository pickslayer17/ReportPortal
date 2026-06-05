using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ReportPortal.DAL;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Boots the real API in-memory (TestServer) but points EF at a dedicated, disposable
/// integration-test database so the suite creates and tears down its own data.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    public const string TestConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=ReportPortal_IntegrationTests;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            // Drop the app's DbContext wiring and repoint it at the test database.
            services.RemoveAll<DbContextOptions<ApplicationContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<ApplicationContext>();

            services.AddDbContext<ApplicationContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(TestConnectionString));
        });
    }

    /// <summary>Recreates the test database schema from scratch (fresh, single migration applied).</summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    public async Task DropDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await db.Database.EnsureDeletedAsync();
    }
}
