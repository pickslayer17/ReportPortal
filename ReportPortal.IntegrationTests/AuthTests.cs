using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
public class AuthTests : IntegrationTestBase
{
    [Test]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var res = await Client.ApiPost("/api/UserManagement/Login", body: new { email = AdminEmail, password = AdminPassword });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Token(), Is.Not.Empty);
    }

    [Test]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
    {
        var res = await Client.ApiPost("/api/UserManagement/Login", body: new { email = "nobody@test.com", password = "whatever1" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var res = await Client.ApiPost("/api/UserManagement/Login", body: new { email = AdminEmail, password = "wrongpass" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Me_ReturnsCurrentUser()
    {
        var res = await Client.ApiGet("/api/UserManagement/me", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
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
        await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");
        var res = await Client.ApiPost("/api/UserManagement/me/change-password", tok, new { currentPassword = "WRONG", newPassword = "newpass12" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task ChangePassword_Valid_AllowsReloginWithNewPassword()
    {
        await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");

        var change = await Client.ApiPost("/api/UserManagement/me/change-password", tok, new { currentPassword = "bobpass12", newPassword = "bobpass34" });
        Assert.That(change.Status, Is.EqualTo(HttpStatusCode.OK));

        var relogin = await Client.ApiPost("/api/UserManagement/Login", body: new { email = "bob@test.com", password = "bobpass34" });
        Assert.That(relogin.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateProfile_ChangesEmail()
    {
        await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");
        var res = await Client.ApiPut("/api/UserManagement/me", tok, new { email = "bob2@test.com" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task UpdateProfile_ToTakenEmail_Conflict()
    {
        await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");
        var res = await Client.ApiPut("/api/UserManagement/me", tok, new { email = AdminEmail });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Conflict));
    }
}
