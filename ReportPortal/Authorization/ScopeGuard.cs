using System.Security.Claims;
using ReportPortal.Constants;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.Authorization
{
    /// <summary>
    /// Tenant-scope check for endpoints whose resource id lives in the request body (and so
    /// cannot be caught by <see cref="ProjectScopeFilter"/>, which only sees route values).
    /// Admins bypass; non-members get a <see cref="ForbiddenAccessException"/> (mapped to 403).
    /// </summary>
    public static class ScopeGuard
    {
        public static async Task EnsureAccessAsync(ClaimsPrincipal user, IProjectScopeRepository scope,
            ScopeResource resource, int id, CancellationToken cancellationToken = default)
        {
            if (user.IsInRole(UserRoles.Admin)) return;

            if (!int.TryParse(user.FindFirst("UserId")?.Value, out var userId))
                throw new ForbiddenAccessException("No authenticated user.");

            var projectId = await scope.ResolveProjectIdAsync(resource, id, cancellationToken);
            if (projectId == null || !await scope.IsMemberAsync(userId, projectId.Value, cancellationToken))
                throw new ForbiddenAccessException("You do not have access to this resource.");
        }
    }
}
