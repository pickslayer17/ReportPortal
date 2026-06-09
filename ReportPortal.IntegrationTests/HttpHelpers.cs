using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ReportPortal.IntegrationTests;

public record ApiResponse(HttpStatusCode Status, string Body)
{
    private JsonElement Root => JsonDocument.Parse(Body).RootElement;
    public int Id() => Root.GetProperty("id").GetInt32();
    public int Int(string property) => Root.GetProperty(property).GetInt32();
    public string Token() => Root.GetProperty("token").GetString()!;
    public int ArrayCount() => Root.GetArrayLength();
    public IEnumerable<int> Ids() => Root.EnumerateArray().Select(e => e.GetProperty("id").GetInt32());
    public IEnumerable<int> IntsOf(string property) => Root.EnumerateArray().Select(e => e.GetProperty(property).GetInt32());
    public IEnumerable<string> Names() => Root.EnumerateArray().Select(e => e.GetProperty("name").GetString()!);
}

public static class HttpHelpers
{
    public static async Task<ApiResponse> Api(this HttpClient client, HttpMethod method, string path, string? token = null, object? body = null)
    {
        using var req = new HttpRequestMessage(method, path);
        if (token != null) req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body != null) req.Content = JsonContent.Create(body);
        using var resp = await client.SendAsync(req);
        return new ApiResponse(resp.StatusCode, await resp.Content.ReadAsStringAsync());
    }

    public static Task<ApiResponse> ApiGet(this HttpClient c, string path, string? token = null) => c.Api(HttpMethod.Get, path, token);
    public static Task<ApiResponse> ApiPost(this HttpClient c, string path, string? token = null, object? body = null) => c.Api(HttpMethod.Post, path, token, body);
    public static Task<ApiResponse> ApiPut(this HttpClient c, string path, string? token = null, object? body = null) => c.Api(HttpMethod.Put, path, token, body);
    public static Task<ApiResponse> ApiDelete(this HttpClient c, string path, string? token = null) => c.Api(HttpMethod.Delete, path, token);

    public static async Task<string> LoginAsync(this HttpClient c, string email, string password)
        => (await c.ApiPost("/api/UserManagement/Login", body: new { email, password })).Token();

    /// <summary>Posts a single file as multipart/form-data (the shape the upload-trx endpoint expects).</summary>
    public static async Task<ApiResponse> ApiUploadFile(this HttpClient client, string path, string? token, string filePath, string formField = "file")
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, path);
        if (token != null) req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, formField, Path.GetFileName(filePath));
        req.Content = content;

        using var resp = await client.SendAsync(req);
        return new ApiResponse(resp.StatusCode, await resp.Content.ReadAsStringAsync());
    }
}
