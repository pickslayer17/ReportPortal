using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
public class UserManagementTests : IntegrationTestBase
{
    [Test]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", AdminToken, new { email = "new@test.com", password = "password1", userRole = RoleUser });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Id(), Is.GreaterThan(0));
    }

    [Test]
    public async Task CreateUser_DuplicateEmail_Conflict()
    {
        await Data.CreateUserAsync("dup@test.com", "password1");
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", AdminToken, new { email = "dup@test.com", password = "password1", userRole = RoleUser });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task CreateUser_InvalidEmail_BadRequest()
    {
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", AdminToken, new { email = "notanemail", password = "password1", userRole = RoleUser });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CreateUser_ShortPassword_BadRequest()
    {
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", AdminToken, new { email = "x@test.com", password = "short", userRole = RoleUser });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetUsers_NonAdmin_WithoutSharedProject_ReturnsOnlySelf()
    {
        var bob = await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");

        var res = await Client.ApiGet("/api/UserManagement/GetUsers", tok);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { bob }));
    }

    [Test]
    public async Task GetUsers_NonAdmin_IncludesColleagueSharingProject()
    {
        var bob = await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var carol = await Data.CreateUserAsync("carol@test.com", "carolpass1");
        var project = await Data.CreateProjectAsync("Shared");
        await Data.AddProjectMemberAsync(project, bob);
        await Data.AddProjectMemberAsync(project, carol);

        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");
        var res = await Client.ApiGet("/api/UserManagement/GetUsers", tok);
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { bob, carol }));
    }

    [Test]
    public async Task GetAllProjectsUsers_Admin_ReturnsEveryone()
    {
        await Data.CreateUserAsync("bob@test.com", "bobpass12");
        await Data.CreateUserAsync("carol@test.com", "carolpass1");

        var res = await Client.ApiGet("/api/UserManagement/GetAllProjectsUsers", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.ArrayCount(), Is.EqualTo(3), "admin + bob + carol");
    }
}
