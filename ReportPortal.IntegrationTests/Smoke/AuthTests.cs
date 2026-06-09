using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
[Category("Smoke")]
public class AuthTests : SmokeTestBase
{
    [Test]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var res = await Client.ApiPost("/api/UserManagement/Login",
            body: new { email = TestCatalog.AdminEmail, password = TestCatalog.AdminPassword });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Token(), Is.Not.Empty);
    }

    [Test]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
    {
        var res = await Client.ApiPost("/api/UserManagement/Login",
            body: new { email = UniqueEmail("nobody"), password = "whatever1" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var res = await Client.ApiPost("/api/UserManagement/Login",
            body: new { email = TestCatalog.AdminEmail, password = "wrongpass" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Me_ReturnsCurrentUser()
    {
        var res = await Client.ApiGet("/api/UserManagement/me", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Body, Does.Contain(TestCatalog.AdminEmail), "the current user is the admin");
    }

    [Test]
    public async Task ValidateToken_WithToken_Ok()
        => Assert.That((await Client.ApiGet("/api/UserManagement/ValidateToken", AdminToken)).Status, Is.EqualTo(HttpStatusCode.OK));

    [Test]
    public async Task ValidateToken_WithoutToken_Unauthorized()
        => Assert.That((await Client.ApiGet("/api/UserManagement/ValidateToken")).Status, Is.EqualTo(HttpStatusCode.Unauthorized));

    [Test]
    public async Task ChangePassword_WithWrongCurrent_BadRequest()
    {
        var email = UniqueEmail("bob");
        await Data.CreateUserAsync(email, "bobpass12");
        var tok = await Client.LoginAsync(email, "bobpass12");
        var res = await Client.ApiPost("/api/UserManagement/me/change-password", tok, new { currentPassword = "WRONG", newPassword = "newpass12" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task ChangePassword_Valid_AllowsReloginWithNewPassword()
    {
        var email = UniqueEmail("bob");
        await Data.CreateUserAsync(email, "bobpass12");
        var tok = await Client.LoginAsync(email, "bobpass12");

        var change = await Client.ApiPost("/api/UserManagement/me/change-password", tok, new { currentPassword = "bobpass12", newPassword = "bobpass34" });
        Assert.That(change.Status, Is.EqualTo(HttpStatusCode.OK));

        var relogin = await Client.ApiPost("/api/UserManagement/Login", body: new { email, password = "bobpass34" });
        Assert.That(relogin.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateProfile_ChangesEmail()
    {
        var email = UniqueEmail("bob");
        var newEmail = UniqueEmail("bob2");
        await Data.CreateUserAsync(email, "bobpass12");
        var tok = await Client.LoginAsync(email, "bobpass12");

        var res = await Client.ApiPut("/api/UserManagement/me", tok, new { email = newEmail });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That((await Client.ApiGet("/api/UserManagement/me", tok)).Body, Does.Contain(newEmail), "email was updated");
    }

    [Test]
    public async Task UpdateProfile_ToTakenEmail_Conflict()
    {
        var email = UniqueEmail("bob");
        await Data.CreateUserAsync(email, "bobpass12");
        var tok = await Client.LoginAsync(email, "bobpass12");
        var res = await Client.ApiPut("/api/UserManagement/me", tok, new { email = TestCatalog.AdminEmail });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Conflict));
    }
}
