namespace ReportPortal.IntegrationTests;

/// <summary>
/// Well-known names/credentials of the shared baseline that the Initialize category creates and
/// every Smoke/SmokeE2E test relies on. Smoke tests resolve these entities by name at run time,
/// so the values must stay in sync between Initialize (writer) and the base (reader).
/// </summary>
public static class TestCatalog
{
    public const string AdminEmail = "admin@admin.com";
    public const string AdminPassword = "admin123";

    public const string MainUserEmail = "mainuser@dealcloud.com";
    public const string MainUserPassword = "mainpass123";

    public const string MainProjectName = "Main Project";
}
