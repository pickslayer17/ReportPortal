using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
[Category("Smoke")]
public class ProjectTests : SmokeTestBase
{
    [Test]
    public async Task AddProject_Admin_Ok()
        => Assert.That((await Client.ApiPost("/api/ProjectManagement/AddProject", AdminToken, new { name = Unique("P") })).Status, Is.EqualTo(HttpStatusCode.OK));

    [Test]
    public async Task AddProject_NonAdmin_Forbidden()
    {
        var email = UniqueEmail("bob");
        await Data.CreateUserAsync(email, "bobpass12");
        var tok = await Client.LoginAsync(email, "bobpass12");
        Assert.That((await Client.ApiPost("/api/ProjectManagement/AddProject", tok, new { name = Unique("Hack") })).Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetAllProjects_NonAdmin_ReturnsOnlyMemberProjects()
    {
        var email = UniqueEmail("bob");
        var bob = await Data.CreateUserAsync(email, "bobpass12");
        var mine = await Data.CreateProjectAsync(Unique("Mine"));
        await Data.CreateProjectAsync(Unique("NotMine"));
        await Data.AddProjectMemberAsync(mine, bob);

        var tok = await Client.LoginAsync(email, "bobpass12");
        var res = await Client.ApiGet("/api/ProjectManagement/GetAllProject", tok);
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { mine }), "non-admin sees only projects they belong to");
    }

    [Test]
    public async Task GetAllProjects_Admin_SeesEveryProjectItCreated()
    {
        var a = await Data.CreateProjectAsync(Unique("A"));
        var b = await Data.CreateProjectAsync(Unique("B"));
        var res = await Client.ApiGet("/api/ProjectManagement/GetAllProject", AdminToken);
        Assert.That(res.Ids(), Is.SupersetOf(new[] { a, b }), "admin sees all projects");
    }

    [Test]
    public async Task DeleteProject_Admin_Ok()
    {
        var p = await Data.CreateProjectAsync(Unique("ToDelete"));
        Assert.That((await Client.ApiPost($"/api/ProjectManagement/DeleteProject/{p}", AdminToken)).Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Membership_Add_List_Remove()
    {
        var bob = await Data.CreateUserAsync(UniqueEmail("bob"), "bobpass12");
        var p = await Data.CreateProjectAsync(Unique("P"));

        var add = await Client.ApiPost($"/api/ProjectManagement/{p}/members/{bob}", AdminToken);
        Assert.That(add.Status, Is.EqualTo(HttpStatusCode.OK));

        var list = await Client.ApiGet($"/api/ProjectManagement/{p}/members", AdminToken);
        Assert.That(list.Ids(), Is.EquivalentTo(new[] { bob }));

        var remove = await Client.ApiDelete($"/api/ProjectManagement/{p}/members/{bob}", AdminToken);
        Assert.That(remove.Status, Is.EqualTo(HttpStatusCode.OK));

        var after = await Client.ApiGet($"/api/ProjectManagement/{p}/members", AdminToken);
        Assert.That(after.ArrayCount(), Is.EqualTo(0));
    }

    [Test]
    public async Task AddMember_UnknownProject_NotFound()
        => Assert.That((await Client.ApiPost("/api/ProjectManagement/99999999/members/1", AdminToken)).Status, Is.EqualTo(HttpStatusCode.NotFound));

    [Test]
    public async Task AddMember_UnknownUser_NotFound()
    {
        var p = await Data.CreateProjectAsync(Unique("P"));
        Assert.That((await Client.ApiPost($"/api/ProjectManagement/{p}/members/99999999", AdminToken)).Status, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
