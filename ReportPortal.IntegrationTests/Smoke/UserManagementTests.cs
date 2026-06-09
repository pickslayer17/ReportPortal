using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
[Category("Smoke")]
public class UserManagementTests : SmokeTestBase
{
    [Test]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", AdminToken, new { email = UniqueEmail("new"), password = "password1", userRole = RoleUser });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Id(), Is.GreaterThan(0));
    }

    [Test]
    public async Task CreateUser_DuplicateEmail_Conflict()
    {
        var email = UniqueEmail("dup");
        await Data.CreateUserAsync(email, "password1");
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", AdminToken, new { email, password = "password1", userRole = RoleUser });
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
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", AdminToken, new { email = UniqueEmail("x"), password = "short", userRole = RoleUser });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetUsers_NonAdmin_WithoutSharedProject_ReturnsOnlySelf()
    {
        var email = UniqueEmail("bob");
        var bob = await Data.CreateUserAsync(email, "bobpass12");
        var tok = await Client.LoginAsync(email, "bobpass12");

        var res = await Client.ApiGet("/api/UserManagement/GetUsers", tok);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { bob }), "no shared project -> sees only self");
    }

    [Test]
    public async Task GetUsers_NonAdmin_IncludesColleagueSharingProject()
    {
        var bobEmail = UniqueEmail("bob");
        var bob = await Data.CreateUserAsync(bobEmail, "bobpass12");
        var carol = await Data.CreateUserAsync(UniqueEmail("carol"), "carolpass1");
        var project = await Data.CreateProjectAsync(Unique("Shared"));
        await Data.AddProjectMemberAsync(project, bob);
        await Data.AddProjectMemberAsync(project, carol);

        var tok = await Client.LoginAsync(bobEmail, "bobpass12");
        var res = await Client.ApiGet("/api/UserManagement/GetUsers", tok);
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { bob, carol }), "sees self + project colleague only");
    }

    [Test]
    public async Task GetAllProjectsUsers_Admin_SeesEveryUserItCreated()
    {
        var bob = await Data.CreateUserAsync(UniqueEmail("bob"), "bobpass12");
        var carol = await Data.CreateUserAsync(UniqueEmail("carol"), "carolpass1");

        var res = await Client.ApiGet("/api/UserManagement/GetAllProjectsUsers", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Ids(), Is.SupersetOf(new[] { bob, carol }), "admin sees every user");
    }
}
