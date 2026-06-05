using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Base for "always relevant" API tests. Before every test it wipes the database and bootstraps
/// a fresh admin (we own the test DB), so tests never depend on leftover state or on first-deploy
/// conditions. Bootstrap-only behaviour (SetupAdmin on an empty system) lives in the Bootstrap
/// category and does NOT use this base.
/// </summary>
public abstract class IntegrationTestBase
{
    protected const string AdminEmail = "admin@admin.com";
    protected const string AdminPassword = "admin123";

    protected const int RoleUser = 0;
    protected const int RoleAdmin = 1;
    protected const int OutcomePassed = 0;
    protected const int OutcomeFailed = 1;
    protected const int OutcomeNotRun = 2;
    protected const int ReviewOutcomeNotRepro = 1;
    protected const int ReviewOutcomeProductBug = 2;

    protected HttpClient Client = null!;
    protected string AdminToken = null!;
    protected DataBuilder Data = null!;

    [SetUp]
    public async Task BaseSetUp()
    {
        await TestServerSetup.Factory.ResetAsync();
        Client = TestServerSetup.Factory.CreateClient();

        var setup = await Client.ApiPost("/api/UserManagement/SetupAdmin",
            body: new { email = AdminEmail, password = AdminPassword });
        Assert.That(setup.Status, Is.EqualTo(HttpStatusCode.OK), "bootstrap admin for the test");

        AdminToken = await Client.LoginAsync(AdminEmail, AdminPassword);
        Data = new DataBuilder(Client, AdminToken);
    }

    [TearDown]
    public void BaseTearDown() => Client?.Dispose();
}
