using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// The Initialize category: builds the shared baseline FROM SCRATCH and LEAVES it in the database.
/// Its [OneTimeSetUp] drops the DB (if any) and recreates the schema from the model, so this run
/// also doubles as a deploy-readiness check ("does everything stand up on a clean database?").
/// Run it once, then run Smoke / SmokeE2E against the result as many times as you like:
///   dotnet test --filter "Category=Initialize"
///   dotnet test --filter "Category=Smoke"   (repeatable)
/// If any step here fails, don't run the other categories — the baseline they depend on is broken.
/// This is the ONLY fixture that wipes and recreates the schema.
/// </summary>
[TestFixture]
[Category("Initialize")]
public class InitializeTests
{
    private static HttpClient _client = null!;
    private static string _adminToken = null!;
    private static int _mainProjectId;
    private static int _mainUserId;

    [OneTimeSetUp]
    public async Task BuildFreshDatabase()
    {
        await TestServerSetup.Factory.CreateSchemaAsync(); // drop + recreate from the model
        _client = TestServerSetup.Factory.CreateClient();
    }

    [OneTimeTearDown]
    public void Cleanup() => _client?.Dispose();

    [Test, Order(1)]
    public async Task Step1_FirstAdmin_IsCreatedAndCanLogIn()
    {
        var setup = await _client.ApiPost("/api/UserManagement/SetupAdmin",
            body: new { email = TestCatalog.AdminEmail, password = TestCatalog.AdminPassword });
        Assert.That(setup.Status, Is.EqualTo(HttpStatusCode.OK), "bootstrap the very first admin");

        _adminToken = await _client.LoginAsync(TestCatalog.AdminEmail, TestCatalog.AdminPassword);
        Assert.That(_adminToken, Is.Not.Empty, "admin must be able to log in");
    }

    [Test, Order(2)]
    public async Task Step2_Admin_CreatesMainProject()
    {
        var res = await _client.ApiPost("/api/ProjectManagement/AddProject", _adminToken,
            new { name = TestCatalog.MainProjectName });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        _mainProjectId = res.Id();
        Assert.That(_mainProjectId, Is.GreaterThan(0));
    }

    [Test, Order(3)]
    public async Task Step3_Admin_CreatesMainUser_AndAddsToProject()
    {
        var create = await _client.ApiPost("/api/UserManagement/CreateUser", _adminToken,
            new { email = TestCatalog.MainUserEmail, password = TestCatalog.MainUserPassword, userRole = 0 });
        Assert.That(create.Status, Is.EqualTo(HttpStatusCode.OK));
        _mainUserId = create.Id();

        var member = await _client.ApiPost($"/api/ProjectManagement/{_mainProjectId}/members/{_mainUserId}", _adminToken);
        Assert.That(member.Status, Is.EqualTo(HttpStatusCode.OK), "main user joins the main project");
    }
}
