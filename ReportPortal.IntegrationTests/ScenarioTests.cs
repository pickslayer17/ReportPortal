using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// End-to-end API scenario over the real app + a disposable test database. Mirrors the manual
/// walkthrough: 2 projects, 3 users, a subproject, a run with a folder tree and 3 tests,
/// reviews, project-scoped user listing, subproject-scoped reviewer rule, and cross-project denial.
/// Tests are ordered and share state (one fixture instance) so the scenario builds up step by step.
/// </summary>
[TestFixture]
public class ScenarioTests
{
    private ApiFactory _factory = null!;
    private HttpClient _client = null!;

    private string _admin = null!;
    private string _tokA = null!;
    private string _tokB = null!;
    private int _uA, _uB, _uC, _pA, _pB, _subA, _runId, _tPass, _tFail, _tOther, _rev;

    private const int RoleUser = 0;
    private const int OutcomePassed = 0, OutcomeFailed = 1, OutcomeNotRun = 2;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _factory = new ApiFactory();
        await _factory.ResetDatabaseAsync();           // fresh schema from the single migration
        _client = _factory.CreateClient();

        var setup = await _client.ApiPost("/api/UserManagement/SetupAdmin",
            body: new { email = "admin@admin.com", password = "admin123" });
        Assert.That(setup.Status, Is.EqualTo(HttpStatusCode.OK), "SetupAdmin should bootstrap the first admin");

        _admin = await _client.LoginAsync("admin@admin.com", "admin123");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _factory.DropDatabaseAsync();
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    // ---------- setup steps ----------

    [Test, Order(1)]
    public async Task S01_Admin_creates_three_users()
    {
        var a = await _client.ApiPost("/api/UserManagement/CreateUser", _admin, new { email = "usera@test.com", password = "passa123", userRole = RoleUser });
        var b = await _client.ApiPost("/api/UserManagement/CreateUser", _admin, new { email = "userb@test.com", password = "passb123", userRole = RoleUser });
        var c = await _client.ApiPost("/api/UserManagement/CreateUser", _admin, new { email = "userc@test.com", password = "passc123", userRole = RoleUser });
        Assert.That(new[] { a.Status, b.Status, c.Status }, Is.All.EqualTo(HttpStatusCode.OK));
        _uA = a.Id(); _uB = b.Id(); _uC = c.Id();
    }

    [Test, Order(2)]
    public async Task S02_Admin_creates_two_projects()
    {
        var a = await _client.ApiPost("/api/ProjectManagement/AddProject", _admin, new { name = "ProjA" });
        var b = await _client.ApiPost("/api/ProjectManagement/AddProject", _admin, new { name = "ProjB" });
        Assert.That(new[] { a.Status, b.Status }, Is.All.EqualTo(HttpStatusCode.OK));
        _pA = a.Id(); _pB = b.Id();
    }

    [Test, Order(3)]
    public async Task S03_Assign_users_to_different_projects()
    {
        var a = await _client.ApiPost($"/api/ProjectManagement/{_pA}/members/{_uA}", _admin);
        var b = await _client.ApiPost($"/api/ProjectManagement/{_pB}/members/{_uB}", _admin);
        Assert.That(new[] { a.Status, b.Status }, Is.All.EqualTo(HttpStatusCode.OK));
    }

    [Test, Order(4)]
    public async Task S04_Create_subproject_in_projectA_and_add_userA()
    {
        var sub = await _client.ApiPost($"/api/SubprojectManagement/Project/{_pA}/subprojects", _admin, new { name = "SubA1" });
        Assert.That(sub.Status, Is.EqualTo(HttpStatusCode.OK));
        _subA = sub.Id();

        var member = await _client.ApiPost($"/api/SubprojectManagement/Subproject/{_subA}/members/{_uA}", _admin);
        Assert.That(member.Status, Is.EqualTo(HttpStatusCode.OK), "userA joins the subproject (reviewer-eligible)");
    }

    [Test, Order(5)]
    public async Task S05_Create_run_in_subproject()
    {
        var run = await _client.ApiPost("/api/RunManagement/AddRun", _admin, new { name = "Run 1", subprojectId = _subA });
        Assert.That(run.Status, Is.EqualTo(HttpStatusCode.OK));
        _runId = run.Id();
    }

    [Test, Order(6)]
    public async Task S06_Create_folder_tree_and_three_tests_without_screenshots()
    {
        // common -> groupb -> leaf : 3 tests; common -> groupa : 1 marker so the sibling folder exists
        var p = await _client.ApiPost("/api/TestManagement/AddTest", _admin, new { path = "common.groupb.leaf", runId = _runId, name = "test_pass" });
        var f = await _client.ApiPost("/api/TestManagement/AddTest", _admin, new { path = "common.groupb.leaf", runId = _runId, name = "test_fail" });
        var o = await _client.ApiPost("/api/TestManagement/AddTest", _admin, new { path = "common.groupb.leaf", runId = _runId, name = "test_other" });
        var m = await _client.ApiPost("/api/TestManagement/AddTest", _admin, new { path = "common.groupa", runId = _runId, name = "marker" });
        Assert.That(new[] { p.Status, f.Status, o.Status, m.Status }, Is.All.EqualTo(HttpStatusCode.OK));
        _tPass = p.Id(); _tFail = f.Id(); _tOther = o.Id();

        var r1 = await _client.ApiPost($"/api/TestResultManagement/test/{_tPass}/AddTestResult", _admin, new { testOutcome = OutcomePassed });
        var r2 = await _client.ApiPost($"/api/TestResultManagement/test/{_tFail}/AddTestResult", _admin, new { testOutcome = OutcomeFailed });
        var r3 = await _client.ApiPost($"/api/TestResultManagement/test/{_tOther}/AddTestResult", _admin, new { testOutcome = OutcomeNotRun });
        Assert.That(new[] { r1.Status, r2.Status, r3.Status }, Is.All.EqualTo(HttpStatusCode.OK), "results add without screenshots");
    }

    // ---------- walk as userA (member of ProjA + SubA1) ----------

    [Test, Order(10)]
    public async Task S10_UserA_logs_in()
    {
        _tokA = await _client.LoginAsync("usera@test.com", "passa123");
        Assert.That(_tokA, Is.Not.Null.And.Not.Empty);
    }

    [Test, Order(11)]
    public async Task S11_UserA_can_get_run()
        => Assert.That((await _client.ApiGet($"/api/RunManagement/Runs/{_runId}", _tokA)).Status, Is.EqualTo(HttpStatusCode.OK));

    [Test, Order(12)]
    public async Task S12_UserA_sees_expected_folder_tree()
    {
        var res = await _client.ApiGet($"/api/FolderManagement/Runs/{_runId}/folders", _tokA);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        var names = res.Names().ToList();
        Assert.That(names, Does.Contain("common").And.Contains("groupa").And.Contains("groupb").And.Contains("leaf"));
    }

    [Test, Order(13)]
    public async Task S13_UserA_can_get_folder_stats()
        => Assert.That((await _client.ApiGet($"/api/TestManagement/Runs/{_runId}/folder-stats", _tokA)).Status, Is.EqualTo(HttpStatusCode.OK));

    [Test, Order(14)]
    public async Task S14_UserA_can_get_all_run_tests()
    {
        var res = await _client.ApiGet($"/api/TestManagement/Runs/{_runId}/tests", _tokA);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.ArrayCount(), Is.EqualTo(4), "3 tests in leaf + 1 marker");
    }

    [Test, Order(15)]
    public async Task S15_UserA_sets_review_outcome_and_comments()
    {
        var review = await _client.ApiGet($"/api/TestReviewManagement/test/{_tFail}/TestReview", _tokA);
        Assert.That(review.Status, Is.EqualTo(HttpStatusCode.OK));
        _rev = review.Id();

        var outcome = await _client.ApiPut($"/api/TestReviewManagement/TestReview/{_rev}/UpdateOutcome", _tokA, new { id = _rev, testReviewOutcome = OutcomeNotRun });
        var comments = await _client.ApiPut($"/api/TestReviewManagement/TestReview/{_rev}/UpdateComments", _tokA, new { id = _rev, comments = "looked into it" });
        Assert.That(new[] { outcome.Status, comments.Status }, Is.All.EqualTo(HttpStatusCode.OK));
    }

    [Test, Order(16)]
    public async Task S16_UserA_can_assign_self_as_reviewer_member_of_subproject()
        => Assert.That((await _client.ApiPut($"/api/TestReviewManagement/TestReview/{_rev}/UpdateReviewer/{_uA}", _tokA)).Status, Is.EqualTo(HttpStatusCode.OK));

    [Test, Order(17)]
    public async Task S17_UserA_GetUsers_returns_only_project_colleagues_self()
    {
        var res = await _client.ApiGet("/api/UserManagement/GetUsers", _tokA);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { _uA }), "no shared project with anyone yet -> only self");
    }

    [Test, Order(18)]
    public async Task S18_After_adding_userC_to_project_GetUsers_returns_two()
    {
        var add = await _client.ApiPost($"/api/ProjectManagement/{_pA}/members/{_uC}", _admin);
        Assert.That(add.Status, Is.EqualTo(HttpStatusCode.OK));

        var res = await _client.ApiGet("/api/UserManagement/GetUsers", _tokA);
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { _uA, _uC }), "userA now shares ProjA with userC");
    }

    [Test, Order(19)]
    public async Task S19_Assigning_userC_reviewer_fails_when_not_subproject_member()
    {
        var res = await _client.ApiPut($"/api/TestReviewManagement/TestReview/{_rev}/UpdateReviewer/{_uC}", _tokA);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.BadRequest), "reviewer must belong to the subproject");
    }

    [Test, Order(20)]
    public async Task S20_Assigning_userC_reviewer_succeeds_after_subproject_membership()
    {
        var join = await _client.ApiPost($"/api/SubprojectManagement/Subproject/{_subA}/members/{_uC}", _admin);
        Assert.That(join.Status, Is.EqualTo(HttpStatusCode.OK));

        var res = await _client.ApiPut($"/api/TestReviewManagement/TestReview/{_rev}/UpdateReviewer/{_uC}", _tokA);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    // ---------- cross-project denial: userB (ProjB) hitting ProjA resources ----------

    [Test, Order(30)]
    public async Task S30_UserB_logs_in()
    {
        _tokB = await _client.LoginAsync("userb@test.com", "passb123");
        Assert.That(_tokB, Is.Not.Null.And.Not.Empty);
    }

    [Test, Order(31)]
    public async Task S31_UserB_is_denied_on_projectA_resources()
    {
        var run = await _client.ApiGet($"/api/RunManagement/Runs/{_runId}", _tokB);
        var subRuns = await _client.ApiGet($"/api/RunManagement/Subproject/{_subA}/Runs", _tokB);
        var folders = await _client.ApiGet($"/api/FolderManagement/Runs/{_runId}/folders", _tokB);
        var tests = await _client.ApiGet($"/api/TestManagement/Runs/{_runId}/tests", _tokB);
        var testById = await _client.ApiGet($"/api/TestManagement/tests/{_tFail}", _tokB);
        var results = await _client.ApiGet($"/api/TestResultManagement/test/{_tFail}/TestResults", _tokB);
        var review = await _client.ApiGet($"/api/TestReviewManagement/test/{_tFail}/TestReview", _tokB);
        var project = await _client.ApiGet($"/api/ProjectManagement/GetProject/{_pA}", _tokB);

        Assert.That(new[] { run.Status, subRuns.Status, folders.Status, tests.Status, testById.Status, results.Status, review.Status, project.Status },
            Is.All.EqualTo(HttpStatusCode.Forbidden), "userB is not a member of ProjA");
    }

    [Test, Order(32)]
    public async Task S32_UserB_GetAllProjects_returns_only_own()
    {
        var res = await _client.ApiGet("/api/ProjectManagement/GetAllProject", _tokB);
        Assert.That(res.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(res.Ids(), Is.EquivalentTo(new[] { _pB }), "userB only sees ProjB");
    }
}
