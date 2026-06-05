using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ReportPortal.Authorization
{
    public static class AuthorizationSetup
    {
        /// <summary>
        /// Registers one authorization policy per permission. A policy succeeds when any of the
        /// caller's role claims grants that permission (per <see cref="RolePermissions"/>).
        /// </summary>
        public static void AddPermissionPolicies(this AuthorizationOptions options)
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission, policy =>
                    policy.RequireAssertion(context =>
                        context.User.FindAll(ClaimTypes.Role)
                            .Any(role => RolePermissions.Grants(role.Value, permission))));
            }
        }
    }
}
