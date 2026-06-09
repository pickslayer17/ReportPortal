using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Base for Smoke / SmokeE2E tests. These run on top of the persisted baseline created by the
/// Initialize category: they do NOT wipe the database. Each test logs in as the existing admin,
/// resolves the shared baseline (main project / subproject / user), and creates its OWN
/// uniquely-named data so repeated runs never collide. Assertions are about a test's own data,
/// not global counts.
/// </summary>
public abstract class SmokeTestBase
{
    // Role/outcome literals mirrored from the API enums for readable test bodies.
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

    // Shared baseline resolved from the Initialize seed.
    protected int MainProjectId;
    protected int MainSubprojectId;
    protected int MainUserId;
    protected string MainUserToken = null!;

    [SetUp]
    public async Task SmokeSetUp()
    {
        Client = TestServerSetup.Factory.CreateClient();

        var login = await Client.ApiPost("/api/UserManagement/Login",
            body: new { email = TestCatalog.AdminEmail, password = TestCatalog.AdminPassword });
        Assert.That(login.Status, Is.EqualTo(HttpStatusCode.OK),
            "Smoke tests run on top of the Initialize baseline. Run it first: " +
            "dotnet test --filter \"Category=Initialize\".");

        AdminToken = login.Token();
        Data = new DataBuilder(Client, AdminToken);

        MainProjectId = (await Client.ApiGet("/api/ProjectManagement/GetAllProject", AdminToken))
            .IdByName(TestCatalog.MainProjectName);
        MainSubprojectId = (await Client.ApiGet($"/api/SubprojectManagement/Project/{MainProjectId}/subprojects", AdminToken))
            .IdByName(TestCatalog.MainSubprojectName);
        MainUserToken = await Client.LoginAsync(TestCatalog.MainUserEmail, TestCatalog.MainUserPassword);
        MainUserId = (await Client.ApiGet("/api/UserManagement/me", MainUserToken)).Id();
    }

    [TearDown]
    public void SmokeTearDown() => Client?.Dispose();

    /// <summary>Unique, collision-free name/email fragment for data this test creates.</summary>
    protected static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";
    protected static string UniqueEmail(string prefix) => $"{prefix}-{Guid.NewGuid():N}@test.com";
}
