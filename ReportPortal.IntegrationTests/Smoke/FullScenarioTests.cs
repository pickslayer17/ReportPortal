using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// One end-to-end smoke "story" covering the whole flow in a single test: 2 projects / 3 users,
/// a subproject, a run with a folder tree and three tests, reviews, project-scoped user listing,
/// the subproject-scoped reviewer rule, and cross-project denial. All entities are uniquely named
/// so the story is self-contained. The focused per-feature fixtures assert the details; this
/// guards the happy path as a whole.
/// </summary>
[TestFixture]
[Category("Smoke")]
public class FullScenarioTests : SmokeTestBase
{
    [Test]
    public async Task Full_project_subproject_run_review_and_isolation_flow()
    {
        // ----- arrange: users, projects, memberships -----
        var emailA = UniqueEmail("usera");
        var emailB = UniqueEmail("userb");
        var userA = await Data.CreateUserAsync(emailA, "passa123");
        var userB = await Data.CreateUserAsync(emailB, "passb123");
        var userC = await Data.CreateUserAsync(UniqueEmail("userc"), "passc123");
        var projA = await Data.CreateProjectAsync(Unique("ProjA"));
        var projB = await Data.CreateProjectAsync(Unique("ProjB"));
        await Data.AddProjectMemberAsync(projA, userA);
        await Data.AddProjectMemberAsync(projB, userB);

        var subA = await Data.CreateSubprojectAsync(projA, Unique("SubA1"));
        await Data.AddSubprojectMemberAsync(subA, userA);

        var runId = await Data.CreateRunAsync(subA, "Run 1");
        var (failTest, _) = await Data.AddTestAsync(runId, "common.groupb.leaf", "test_fail");
        var (passTest, _) = await Data.AddTestAsync(runId, "common.groupb.leaf", "test_pass");
        var (otherTest, _) = await Data.AddTestAsync(runId, "common.groupb.leaf", "test_other");
        await Data.AddTestAsync(runId, "common.groupa", "marker"); // materialises the sibling folder
        await Data.AddResultAsync(passTest, OutcomePassed);
        await Data.AddResultAsync(failTest, OutcomeFailed);
        await Data.AddResultAsync(otherTest, OutcomeNotRun);

        // ----- walk as userA (member of ProjA + SubA1) -----
        var tokA = await Client.LoginAsync(emailA, "passa123");

        Assert.That((await Client.ApiGet($"/api/RunManagement/Runs/{runId}", tokA)).Status, Is.EqualTo(HttpStatusCode.OK));

        var folders = await Client.ApiGet($"/api/FolderManagement/Runs/{runId}/folders", tokA);
        Assert.That(folders.Names(), Does.Contain("common").And.Contains("groupa").And.Contains("groupb").And.Contains("leaf"));

        Assert.That((await Client.ApiGet($"/api/TestManagement/Runs/{runId}/tests", tokA)).ArrayCount(), Is.EqualTo(4));

        var reviewId = await Data.GetReviewIdAsync(failTest);
        Assert.That((await Client.ApiPut($"/api/TestReviewManagement/TestReview/{reviewId}/UpdateOutcome", tokA, new { id = reviewId, testReviewOutcome = ReviewOutcomeNotRepro })).Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That((await Client.ApiPut($"/api/TestReviewManagement/TestReview/{reviewId}/UpdateComments", tokA, new { id = reviewId, comments = "looked into it" })).Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That((await Client.ApiPut($"/api/TestReviewManagement/TestReview/{reviewId}/UpdateReviewer/{userA}", tokA)).Status, Is.EqualTo(HttpStatusCode.OK));

        // project-scoped user listing: only self, then self + userC once shared
        Assert.That((await Client.ApiGet("/api/UserManagement/GetUsers", tokA)).Ids(), Is.EquivalentTo(new[] { userA }));
        await Data.AddProjectMemberAsync(projA, userC);
        Assert.That((await Client.ApiGet("/api/UserManagement/GetUsers", tokA)).Ids(), Is.EquivalentTo(new[] { userA, userC }));

        // subproject-scoped reviewer rule
        Assert.That((await Client.ApiPut($"/api/TestReviewManagement/TestReview/{reviewId}/UpdateReviewer/{userC}", tokA)).Status, Is.EqualTo(HttpStatusCode.BadRequest));
        await Data.AddSubprojectMemberAsync(subA, userC);
        Assert.That((await Client.ApiPut($"/api/TestReviewManagement/TestReview/{reviewId}/UpdateReviewer/{userC}", tokA)).Status, Is.EqualTo(HttpStatusCode.OK));

        // ----- cross-project: userB (ProjB) denied on every ProjA resource -----
        var tokB = await Client.LoginAsync(emailB, "passb123");
        var denied = new[]
        {
            (await Client.ApiGet($"/api/RunManagement/Runs/{runId}", tokB)).Status,
            (await Client.ApiGet($"/api/RunManagement/Subproject/{subA}/Runs", tokB)).Status,
            (await Client.ApiGet($"/api/FolderManagement/Runs/{runId}/folders", tokB)).Status,
            (await Client.ApiGet($"/api/TestManagement/Runs/{runId}/tests", tokB)).Status,
            (await Client.ApiGet($"/api/TestManagement/tests/{failTest}", tokB)).Status,
            (await Client.ApiGet($"/api/TestResultManagement/test/{failTest}/TestResults", tokB)).Status,
            (await Client.ApiGet($"/api/TestReviewManagement/test/{failTest}/TestReview", tokB)).Status,
            (await Client.ApiGet($"/api/ProjectManagement/GetProject/{projA}", tokB)).Status,
        };
        Assert.That(denied, Is.All.EqualTo(HttpStatusCode.Forbidden));

        Assert.That((await Client.ApiGet("/api/ProjectManagement/GetAllProject", tokB)).Ids(), Is.EquivalentTo(new[] { projB }));
    }
}
