using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// End-to-end TRX upload, run on top of the Initialize baseline: the main user (a regular,
/// non-admin project member) uploads a .trx to the main project and we verify the parsed run,
/// folder tree and tests. Covers both upload modes (all tests / failed only) and the
/// HasAnyTests fix (a fully "Completed"/all-passed run must still import). Fixtures live in
/// TestData/ and are copied next to the test assembly (see the .csproj).
/// </summary>
[TestFixture]
[Category("SmokeE2E")]
public class TrxUploadTests : SmokeTestBase
{
    // Real NUnit run: 247 results (125 passed / 117 failed / 5 not-executed) across 11 classes.
    private const string RealTrx = "qaautouser_vmdcadoqa1a4_2026-06-08_21_21_44.trx";
    private const int RealAllTests = 247;
    private const int RealAllFolders = 23;     // root + 22 namespace nodes
    private const int RealFailedTests = 117;
    private const int RealFailedFolders = 16;  // root + 15 namespace nodes that hold a failed test

    // Synthetic minimal run: outcome="Completed", two passed tests in two classes.
    private const string CompletedTrx = "completed-all-passed.trx";
    private const int CompletedAllTests = 2;

    private static string Fixture(string name) => Path.Combine(AppContext.BaseDirectory, "TestData", name);

    private async Task<int> UploadAsync(string fixtureName, bool? failedOnly = null)
    {
        var path = Fixture(fixtureName);
        Assert.That(File.Exists(path), Is.True, $"fixture .trx must be copied to output: {path}");

        var fields = failedOnly is null ? null : new Dictionary<string, string> { ["failedOnly"] = failedOnly.Value.ToString() };
        var upload = await Client.ApiUploadFile(
            $"/api/RunManagement/Project/{MainProjectId}/upload-trx", MainUserToken, path, fields: fields);

        Assert.That(upload.Status, Is.EqualTo(HttpStatusCode.OK), $"upload should succeed; body: {upload.Body}");
        var runId = upload.Int("runId");
        Assert.That(runId, Is.GreaterThan(0), "upload should report the created run id");

        // The new run must show up among the main project's runs.
        var runs = await Client.ApiGet($"/api/RunManagement/Project/{MainProjectId}/Runs", MainUserToken);
        Assert.That(runs.Ids(), Does.Contain(runId), "uploaded run appears in the project run list");
        return runId;
    }

    private Task<ApiResponse> FoldersAsync(int runId) => Client.ApiGet($"/api/FolderManagement/Runs/{runId}/folders", MainUserToken);
    private Task<ApiResponse> TestsAsync(int runId) => Client.ApiGet($"/api/TestManagement/Runs/{runId}/tests", MainUserToken);

    [Test]
    public async Task UploadTrx_AllTests_CreatesFullFolderTreeAndTests()
    {
        var runId = await UploadAsync(RealTrx); // default: import everything

        var folders = await FoldersAsync(runId);
        Assert.That(folders.ArrayCount(), Is.EqualTo(RealAllFolders), "full namespace folder tree (root + nodes)");

        var tests = await TestsAsync(runId);
        Assert.That(tests.ArrayCount(), Is.EqualTo(RealAllTests), "every parsed test stored under the run");

        // No orphans: every test belongs to a folder of this run.
        Assert.That(tests.IntsOf("folderId").Distinct(), Is.SubsetOf(folders.Ids().ToList()));
    }

    [Test]
    public async Task UploadTrx_FailedOnly_ImportsOnlyFailedTests()
    {
        var runId = await UploadAsync(RealTrx, failedOnly: true);

        var tests = await TestsAsync(runId);
        Assert.That(tests.ArrayCount(), Is.EqualTo(RealFailedTests), "only the failed tests are imported");

        var folders = await FoldersAsync(runId);
        Assert.That(folders.ArrayCount(), Is.EqualTo(RealFailedFolders), "only folders holding a failed test (plus root)");
    }

    [Test]
    public async Task UploadCompletedRun_ImportsAllTests_HasAnyTestsFix()
    {
        // Regression guard for the previously-inverted HasAnyTests: a fully "Completed" run
        // (all passed) used to import zero tests. It must now import them.
        var runId = await UploadAsync(CompletedTrx);

        var tests = await TestsAsync(runId);
        Assert.That(tests.ArrayCount(), Is.EqualTo(CompletedAllTests), "a completed all-passed run still imports its tests");
    }

    [Test]
    public async Task UploadCompletedRun_FailedOnly_ImportsNothing()
    {
        var runId = await UploadAsync(CompletedTrx, failedOnly: true);

        var tests = await TestsAsync(runId);
        Assert.That(tests.ArrayCount(), Is.EqualTo(0), "no failed tests -> nothing imported");
    }
}
