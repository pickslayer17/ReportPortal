using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// First-deploy-only behaviour: SetupAdmin bootstraps the very first admin while the system is
/// empty, then locks itself once any user exists. Irrelevant on a populated system, so it is
/// isolated under the "Bootstrap" category and does NOT seed an admin in setup.
/// Run normally with: dotnet test --filter "Category!=Bootstrap"; include only on a fresh deploy.
/// </summary>
[TestFixture]
[Category("Bootstrap")]
public class FirstDeployTests
{
    private HttpClient _client = null!;

    [SetUp]
    public async Task SetUp()
    {
        await TestServerSetup.Factory.ResetAsync();
        _client = TestServerSetup.Factory.CreateClient();
    }

    [TearDown]
    public void TearDown() => _client?.Dispose();

    [Test]
    public async Task SetupAdmin_OnEmptySystem_CreatesFirstAdmin()
    {
        var res = await _client.ApiPost("/api/UserManagement/SetupAdmin", body: new { email = "admin@admin.com", password = "admin123" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task SetupAdmin_WhenAdminAlreadyExists_Conflict()
    {
        await _client.ApiPost("/api/UserManagement/SetupAdmin", body: new { email = "admin@admin.com", password = "admin123" });
        var again = await _client.ApiPost("/api/UserManagement/SetupAdmin", body: new { email = "other@test.com", password = "password1" });
        Assert.That(again.Status, Is.EqualTo(HttpStatusCode.Conflict));
    }
}
