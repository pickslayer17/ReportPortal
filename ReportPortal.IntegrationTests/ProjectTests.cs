using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
public class ProjectTests : IntegrationTestBase
{
    [Test]
    public async Task AddProject_Admin_Ok()
        => Assert.That((await Client.ApiPost("/api/ProjectManagement/AddProject", AdminToken, new { name = "P" })).Status, Is.EqualTo(HttpStatusCode.OK));

    [Test]
    public async Task AddProject_NonAdmin_Forbidden()
    {
        await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");
        Assert.That((await Client.ApiPost("/api/ProjectManagement/AddProject", tok, new { name = "Hack" })).Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetAllProjects_NonAdmin_ReturnsOnlyMemberProjects()
    {
        var bob = await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var mine = await Data.CreateProjectAsync("Mine");
        await Data.CreateProjectAsync("NotMine");
        await Data.AddProjectMemberAsync(mine, bob);

        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");
        var res = await Client.ApiGet("/api/ProjectManagement/GetAllProject", tok);
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { mine }));
    }

    [Test]
    public async Task GetAllProjects_Admin_ReturnsAll()
    {
        await Data.CreateProjectAsync("A");
        await Data.CreateProjectAsync("B");
        var res = await Client.ApiGet("/api/ProjectManagement/GetAllProject", AdminToken);
        Assert.That(res.ArrayCount(), Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteProject_Admin_Ok()
    {
        var p = await Data.CreateProjectAsync("ToDelete");
        Assert.That((await Client.ApiPost($"/api/ProjectManagement/DeleteProject/{p}", AdminToken)).Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Membership_Add_List_Remove()
    {
        var bob = await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var p = await Data.CreateProjectAsync("P");

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
        => Assert.That((await Client.ApiPost("/api/ProjectManagement/99999/members/1", AdminToken)).Status, Is.EqualTo(HttpStatusCode.NotFound));

    [Test]
    public async Task AddMember_UnknownUser_NotFound()
    {
        var p = await Data.CreateProjectAsync("P");
        Assert.That((await Client.ApiPost($"/api/ProjectManagement/{p}/members/99999", AdminToken)).Status, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
