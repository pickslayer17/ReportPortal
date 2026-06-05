using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
public class SubprojectTests : IntegrationTestBase
{
    [Test]
    public async Task Create_Admin_Ok()
    {
        var p = await Data.CreateProjectAsync("P");
        var res = await Client.ApiPost($"/api/SubprojectManagement/Project/{p}/subprojects", AdminToken, new { name = "Sub" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Id(), Is.GreaterThan(0));
    }

    [Test]
    public async Task Create_NonAdmin_Forbidden()
    {
        var p = await Data.CreateProjectAsync("P");
        await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var tok = await Client.LoginAsync("bob@test.com", "bobpass12");
        var res = await Client.ApiPost($"/api/SubprojectManagement/Project/{p}/subprojects", tok, new { name = "Sub" });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task List_ReturnsCreatedSubprojects()
    {
        var p = await Data.CreateProjectAsync("P");
        await Data.CreateSubprojectAsync(p, "S1");
        await Data.CreateSubprojectAsync(p, "S2");

        var res = await Client.ApiGet($"/api/SubprojectManagement/Project/{p}/subprojects", AdminToken);
        Assert.That(res.ArrayCount(), Is.EqualTo(2));
    }

    [Test]
    public async Task AddMember_WhenUserIsProjectMember_Ok()
    {
        var bob = await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var p = await Data.CreateProjectAsync("P");
        var sub = await Data.CreateSubprojectAsync(p, "Sub");
        await Data.AddProjectMemberAsync(p, bob);

        var res = await Client.ApiPost($"/api/SubprojectManagement/Subproject/{sub}/members/{bob}", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));

        var members = await Client.ApiGet($"/api/SubprojectManagement/Subproject/{sub}/members", AdminToken);
        Assert.That(members.Ids(), Is.EquivalentTo(new[] { bob }));
    }

    [Test]
    public async Task AddMember_WhenUserNotInParentProject_Forbidden()
    {
        var bob = await Data.CreateUserAsync("bob@test.com", "bobpass12");
        var p = await Data.CreateProjectAsync("P");
        var sub = await Data.CreateSubprojectAsync(p, "Sub");
        // bob is NOT added to project p

        var res = await Client.ApiPost($"/api/SubprojectManagement/Subproject/{sub}/members/{bob}", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}
