using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

[TestFixture]
[Category("Smoke")]
public class SubprojectTests : SmokeTestBase
{
    [Test]
    public async Task Create_Admin_Ok()
    {
        var p = await Data.CreateProjectAsync(Unique("P"));
        var res = await Client.ApiPost($"/api/SubprojectManagement/Project/{p}/subprojects", AdminToken, new { name = Unique("Sub") });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Id(), Is.GreaterThan(0));
    }

    [Test]
    public async Task Create_NonAdmin_Forbidden()
    {
        var p = await Data.CreateProjectAsync(Unique("P"));
        var email = UniqueEmail("bob");
        await Data.CreateUserAsync(email, "bobpass12");
        var tok = await Client.LoginAsync(email, "bobpass12");
        var res = await Client.ApiPost($"/api/SubprojectManagement/Project/{p}/subprojects", tok, new { name = Unique("Sub") });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task List_ReturnsCreatedSubprojects()
    {
        var p = await Data.CreateProjectAsync(Unique("P"));
        var s1 = await Data.CreateSubprojectAsync(p, Unique("S1"));
        var s2 = await Data.CreateSubprojectAsync(p, Unique("S2"));

        var res = await Client.ApiGet($"/api/SubprojectManagement/Project/{p}/subprojects", AdminToken);
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { s1, s2 }), "the new project holds exactly its two subprojects");
    }

    [Test]
    public async Task AddMember_WhenUserIsProjectMember_Ok()
    {
        var bob = await Data.CreateUserAsync(UniqueEmail("bob"), "bobpass12");
        var p = await Data.CreateProjectAsync(Unique("P"));
        var sub = await Data.CreateSubprojectAsync(p, Unique("Sub"));
        await Data.AddProjectMemberAsync(p, bob);

        var res = await Client.ApiPost($"/api/SubprojectManagement/Subproject/{sub}/members/{bob}", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));

        var members = await Client.ApiGet($"/api/SubprojectManagement/Subproject/{sub}/members", AdminToken);
        Assert.That(members.Ids(), Is.EquivalentTo(new[] { bob }));
    }

    [Test]
    public async Task AddMember_WhenUserNotInParentProject_Forbidden()
    {
        var bob = await Data.CreateUserAsync(UniqueEmail("bob"), "bobpass12");
        var p = await Data.CreateProjectAsync(Unique("P"));
        var sub = await Data.CreateSubprojectAsync(p, Unique("Sub"));
        // bob is NOT added to project p

        var res = await Client.ApiPost($"/api/SubprojectManagement/Subproject/{sub}/members/{bob}", AdminToken);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}
