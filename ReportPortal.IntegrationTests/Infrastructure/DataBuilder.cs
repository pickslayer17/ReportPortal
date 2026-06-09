using System.Text.Json;

namespace ReportPortal.IntegrationTests;

/// <summary>
/// Arrange helpers: create the entities a test needs in one line, as the admin. Keeps the
/// feature fixtures focused on the behaviour under test rather than setup plumbing.
/// </summary>
public class DataBuilder
{
    private readonly HttpClient _client;
    private readonly string _admin;

    public DataBuilder(HttpClient client, string adminToken)
    {
        _client = client;
        _admin = adminToken;
    }

    public async Task<int> CreateUserAsync(string email, string password, int role = 0)
        => (await _client.ApiPost("/api/UserManagement/CreateUser", _admin, new { email, password, userRole = role })).Id();

    public async Task<int> CreateProjectAsync(string name)
        => (await _client.ApiPost("/api/ProjectManagement/AddProject", _admin, new { name })).Id();

    public Task AddProjectMemberAsync(int projectId, int userId)
        => _client.ApiPost($"/api/ProjectManagement/{projectId}/members/{userId}", _admin);

    public async Task<int> CreateRunAsync(int projectId, string name = "Run 1")
        => (await _client.ApiPost("/api/RunManagement/AddRun", _admin, new { name, projectId })).Id();

    public async Task<(int testId, int folderId)> AddTestAsync(int runId, string path, string name)
    {
        var res = await _client.ApiPost("/api/TestManagement/AddTest", _admin, new { path, runId, name });
        var root = JsonDocument.Parse(res.Body).RootElement;
        return (root.GetProperty("id").GetInt32(), root.GetProperty("folderId").GetInt32());
    }

    public Task AddResultAsync(int testId, int outcome)
        => _client.ApiPost($"/api/TestResultManagement/test/{testId}/AddTestResult", _admin, new { testOutcome = outcome });

    public async Task<int> GetReviewIdAsync(int testId)
        => (await _client.ApiGet($"/api/TestReviewManagement/test/{testId}/TestReview", _admin)).Id();
}
