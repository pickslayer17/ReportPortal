using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Authorization "capabilities": role/permission policies (what you may do) and tenant scoping
/// (which project's data you may touch), plus the subproject-scoped reviewer rule. Each test
/// builds its own isolated project graph with unique names, so it is independent of the shared
/// baseline and of other runs.
/// </summary>
[TestFixture]
[Category("Smoke")]
public class CapabilityTests : SmokeTestBase
{
    // Project A with a member (userA, also a subproject member), a run, and one reviewed test.
    private async Task<(int pA, int subA, int runId, int testId, int reviewId, int userA, string tokA)> ArrangeProjectAAsync()
    {
        var emailA = UniqueEmail("usera");
        var userA = await Data.CreateUserAsync(emailA, "passa123");
        var pA = await Data.CreateProjectAsync(Unique("ProjA"));
        await Data.AddProjectMemberAsync(pA, userA);
        var subA = await Data.CreateSubprojectAsync(pA, Unique("SubA"));
        await Data.AddSubprojectMemberAsync(subA, userA);
        var runId = await Data.CreateRunAsync(subA);
        var (testId, _) = await Data.AddTestAsync(runId, "common.leaf", "t1");
        await Data.AddResultAsync(testId, OutcomeFailed);
        var reviewId = await Data.GetReviewIdAsync(testId);
        var tokA = await Client.LoginAsync(emailA, "passa123");
        return (pA, subA, runId, testId, reviewId, userA, tokA);
    }

    [Test]
    public async Task NonAdmin_CannotCreateUser()
    {
        var (_, _, _, _, _, _, tokA) = await ArrangeProjectAAsync();
        var res = await Client.ApiPost("/api/UserManagement/CreateUser", tokA, new { email = UniqueEmail("x"), password = "password1", userRole = RoleUser });
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task ProjectMember_CannotDeleteRun_PolicyIsAdminOnly()
    {
        var (_, _, runId, _, _, _, tokA) = await ArrangeProjectAAsync();
        // userA has access to the run (member) but DeleteRuns is an admin-only capability.
        var res = await Client.ApiPost($"/api/RunManagement/Runs/{runId}/delete", tokA);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task Member_CanReadRunResources()
    {
        var (_, subA, runId, testId, _, _, tokA) = await ArrangeProjectAAsync();
        var statuses = new[]
        {
            (await Client.ApiGet($"/api/RunManagement/Runs/{runId}", tokA)).Status,
            (await Client.ApiGet($"/api/RunManagement/Subproject/{subA}/Runs", tokA)).Status,
            (await Client.ApiGet($"/api/FolderManagement/Runs/{runId}/folders", tokA)).Status,
            (await Client.ApiGet($"/api/TestManagement/tests/{testId}", tokA)).Status,
        };
        Assert.That(statuses, Is.All.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task NonMember_IsDeniedAcrossProjectAResources()
    {
        var (pA, subA, runId, testId, _, _, _) = await ArrangeProjectAAsync();

        // userB belongs to a different project only.
        var emailB = UniqueEmail("userb");
        var userB = await Data.CreateUserAsync(emailB, "passb123");
        var pB = await Data.CreateProjectAsync(Unique("ProjB"));
        await Data.AddProjectMemberAsync(pB, userB);
        var tokB = await Client.LoginAsync(emailB, "passb123");

        var statuses = new[]
        {
            (await Client.ApiGet($"/api/RunManagement/Runs/{runId}", tokB)).Status,
            (await Client.ApiGet($"/api/RunManagement/Subproject/{subA}/Runs", tokB)).Status,
            (await Client.ApiGet($"/api/FolderManagement/Runs/{runId}/folders", tokB)).Status,
            (await Client.ApiGet($"/api/TestManagement/Runs/{runId}/tests", tokB)).Status,
            (await Client.ApiGet($"/api/TestManagement/tests/{testId}", tokB)).Status,
            (await Client.ApiGet($"/api/TestResultManagement/test/{testId}/TestResults", tokB)).Status,
            (await Client.ApiGet($"/api/TestReviewManagement/test/{testId}/TestReview", tokB)).Status,
            (await Client.ApiGet($"/api/ProjectManagement/GetProject/{pA}", tokB)).Status,
        };
        Assert.That(statuses, Is.All.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task Reviewer_MustBelongToSubproject()
    {
        var (pA, subA, _, _, reviewId, _, tokA) = await ArrangeProjectAAsync();

        // userC is a project member but NOT a subproject member yet.
        var userC = await Data.CreateUserAsync(UniqueEmail("userc"), "passc123");
        await Data.AddProjectMemberAsync(pA, userC);

        var rejected = await Client.ApiPut($"/api/TestReviewManagement/TestReview/{reviewId}/UpdateReviewer/{userC}", tokA);
        Assert.That(rejected.Status, Is.EqualTo(HttpStatusCode.BadRequest), "not a subproject member yet");

        await Data.AddSubprojectMemberAsync(subA, userC);
        var accepted = await Client.ApiPut($"/api/TestReviewManagement/TestReview/{reviewId}/UpdateReviewer/{userC}", tokA);
        Assert.That(accepted.Status, Is.EqualTo(HttpStatusCode.OK));
    }
}
