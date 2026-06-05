namespace ReportPortal.Authorization
{
    /// <summary>
    /// Discrete permissions used as authorization policy names. Controllers reference
    /// these via [Authorize(Policy = Permissions.X)] instead of hardcoded role strings.
    /// </summary>
    public static class Permissions
    {
        public const string ViewProjects = "ViewProjects";
        public const string ManageProjects = "ManageProjects";
        public const string UploadResults = "UploadResults";
        public const string DeleteRuns = "DeleteRuns";
        public const string ReviewTests = "ReviewTests";
        public const string ManageUsers = "ManageUsers";

        public static readonly IReadOnlyList<string> All = new[]
        {
            ViewProjects,
            ManageProjects,
            UploadResults,
            DeleteRuns,
            ReviewTests,
            ManageUsers
        };
    }
}
