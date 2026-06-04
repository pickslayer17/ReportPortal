namespace ReportPortal.BL.Configuration
{
    /// <summary>
    /// Single source of app configuration. Bound from the "AppSettings" section of
    /// appsettings.json. Everything (Kestrel URL, CORS, DbContext, JWT) reads from here,
    /// so URLs / connection string / secrets live in exactly one place.
    /// </summary>
    public class AppSettings
    {
        public string BackendUrl { get; set; } = string.Empty;
        public string FrontendUrl { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public JwtSettings Jwt { get; set; } = new();
    }

    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
    }
}
