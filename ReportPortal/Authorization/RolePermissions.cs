using ReportPortal.Constants;

namespace ReportPortal.Authorization
{
    /// <summary>
    /// Maps each role (as carried in the JWT role claim) to the set of permissions it grants.
    /// Admin gets everything; a regular User can view, review and upload results, but cannot
    /// manage users/projects or delete structural data (runs/folders/tests).
    /// </summary>
    public static class RolePermissions
    {
        public static readonly IReadOnlyDictionary<string, HashSet<string>> Map =
            new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
            {
                [UserRoles.Admin] = new HashSet<string>
                {
                    Permissions.ViewProjects,
                    Permissions.ManageProjects,
                    Permissions.UploadResults,
                    Permissions.DeleteRuns,
                    Permissions.ReviewTests,
                    Permissions.ManageUsers
                },
                [UserRoles.User] = new HashSet<string>
                {
                    Permissions.ViewProjects,
                    Permissions.UploadResults,
                    Permissions.ReviewTests
                }
            };

        public static bool Grants(string role, string permission) =>
            role != null && Map.TryGetValue(role, out var permissions) && permissions.Contains(permission);
    }
}
