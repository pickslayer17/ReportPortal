using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ReportPortal.Constants;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.Authorization
{
    /// <summary>
    /// Central tenant-scope guard. For any authenticated non-admin request it reads the most
    /// specific resource id from the route, resolves it up to the owning project, and rejects
    /// the request (403) unless the caller is a member of that project. One lookup per request,
    /// independent of how many entities the endpoint returns. Admins and routes without a known
    /// resource id are passed through untouched.
    /// </summary>
    public class ProjectScopeFilter : IAsyncAuthorizationFilter
    {
        // Most specific first: a single request carries exactly one of these.
        private static readonly (string key, ScopeResource resource)[] RouteKeys =
        {
            ("testResultId", ScopeResource.TestResult),
            ("reviewId", ScopeResource.TestReview),
            ("testId", ScopeResource.Test),
            ("folderId", ScopeResource.Folder),
            ("runId", ScopeResource.Run),
            ("projectId", ScopeResource.Project),
        };

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Something earlier already short-circuited (e.g. unauthenticated) — leave it.
            if (context.Result != null) return;

            var user = context.HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated) return;
            if (user.IsInRole(UserRoles.Admin)) return; // admins see everything

            var target = ResolveTarget(context);
            if (target == null) return; // endpoint is not scoped by a route resource id

            if (!int.TryParse(user.FindFirst("UserId")?.Value, out var userId)) return;

            var scope = context.HttpContext.RequestServices.GetRequiredService<IProjectScopeRepository>();
            var projectId = await scope.ResolveProjectIdAsync(target.Value.resource, target.Value.id);
            if (projectId == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            if (!await scope.IsMemberAsync(userId, projectId.Value))
            {
                context.Result = new ForbidResult();
            }
        }

        private static (ScopeResource resource, int id)? ResolveTarget(AuthorizationFilterContext context)
        {
            foreach (var (key, resource) in RouteKeys)
            {
                if (context.RouteData.Values.TryGetValue(key, out var raw)
                    && int.TryParse(raw?.ToString(), out var id))
                {
                    return (resource, id);
                }
            }
            return null;
        }
    }
}
