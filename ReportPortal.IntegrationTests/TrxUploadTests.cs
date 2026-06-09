using System.Net;
using NUnit.Framework;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// End-to-end TRX upload: a regular (non-admin) project member logs in, uploads a real .trx
/// produced by an NUnit Selenium suite, and we verify the upload created a run whose folder
/// tree and tests were populated from the file. The fixture .trx lives in TestData/ and is
/// copied next to the test assembly (see the .csproj), so the test is self-contained.
/// </summary>
[TestFixture]
public class TrxUploadTests : IntegrationTestBase
{
    // Real NUnit run: 247 results across 11 test classes under a single DealCloud.* namespace.
    private const string TrxFileName = "qaautouser_vmdcadoqa1a4_2026-06-08_21_21_44.trx";

    // The upload endpoint names the run after the file (without extension): "<base> (<timestamp> UTC)".
    private const string ExpectedRunNamePrefix = "qaautouser_vmdcadoqa1a4_2026-06-08_21_21_44";

    // Distinct tests in the file = distinct (folder, method-name) pairs; no name collisions here.
    private const int ExpectedTestCount = 247;

    // 22 distinct namespace nodes (DealCloud -> ... -> 11 leaf class folders) plus the run's root folder.
    private const int ExpectedFolderCount = 23;

    private static string TrxFilePath =>
        Path.Combine(AppContext.BaseDirectory, "TestData", TrxFileName);

    [Test]
    public async Task UploadTrx_AsProjectMember_CreatesRunWithFoldersAndTests()
    {
        // Arrange: project + subproject (as admin), and a regular member who will do the upload.
        Assert.That(File.Exists(TrxFilePath), Is.True, $"fixture .trx must be copied to output: {TrxFilePath}");

        var projectId = await Data.CreateProjectAsync("DealCloud");
        var subprojectId = await Data.CreateSubprojectAsync(projectId, "Selenium");

        var userId = await Data.CreateUserAsync("qa@dealcloud.com", "qapass123", RoleUser);
        await Data.AddProjectMemberAsync(projectId, userId);
        var userToken = await Client.LoginAsync("qa@dealcloud.com", "qapass123");

        // Act: upload the .trx as the logged-in member.
        var upload = await Client.ApiUploadFile(
            $"/api/RunManagement/Subproject/{subprojectId}/upload-trx", userToken, TrxFilePath);

        Assert.That(upload.Status, Is.EqualTo(HttpStatusCode.OK), $"upload should succeed; body: {upload.Body}");
        var runId = upload.Int("runId");
        Assert.That(runId, Is.GreaterThan(0), "upload should report the created run id");

        // Assert: the new run shows up among the subproject's runs.
        var runs = await Client.ApiGet($"/api/RunManagement/Subproject/{subprojectId}/Runs", userToken);
        Assert.That(runs.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(runs.Ids(), Does.Contain(runId), "uploaded run must appear in the subproject run list");
        Assert.That(runs.Names().Any(n => n.StartsWith(ExpectedRunNamePrefix)), Is.True,
            "uploaded run should be named after the .trx file");

        // Assert: folders were created (the namespace tree), and tests were placed in them.
        var folders = await Client.ApiGet($"/api/FolderManagement/Runs/{runId}/folders", userToken);
        Assert.That(folders.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(folders.ArrayCount(), Is.EqualTo(ExpectedFolderCount),
            "run should hold the full namespace folder tree (root + nodes)");

        var tests = await Client.ApiGet($"/api/TestManagement/Runs/{runId}/tests", userToken);
        Assert.That(tests.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(tests.ArrayCount(), Is.EqualTo(ExpectedTestCount),
            "every parsed test from the .trx should be stored under the run");

        // Each test must belong to one of the run's folders (no orphans).
        var folderIds = folders.Ids().ToList();
        Assert.That(tests.IntsOf("folderId").Distinct(), Is.SubsetOf(folderIds),
            "every test should be linked to a folder of this run");
    }
}
