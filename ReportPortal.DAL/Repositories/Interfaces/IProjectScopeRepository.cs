namespace ReportPortal.DAL.Repositories.Interfaces
{
    /// <summary>Kinds of resource that can be resolved up to their owning project.</summary>
    public enum ScopeResource
    {
        Project,
        Subproject,
        Run,
        Folder,
        Test,
        TestResult,
        TestReview
    }

    /// <summary>
    /// Resolves any resource to the project that owns it and checks project membership.
    /// Backs the central tenant-scope authorization (one indexed lookup per request).
    /// </summary>
    public interface IProjectScopeRepository
    {
        /// <summary>Returns the owning project id, or null if the resource does not exist.</summary>
        Task<int?> ResolveProjectIdAsync(ScopeResource resource, int id, CancellationToken cancellationToken = default);

        Task<bool> IsMemberAsync(int userId, int projectId, CancellationToken cancellationToken = default);

        /// <summary>Subproject id that owns a review (review -> test -> run -> subproject), or null.</summary>
        Task<int?> ResolveSubprojectIdForReviewAsync(int reviewId, CancellationToken cancellationToken = default);

        /// <summary>Subproject-level membership (governs reviewer eligibility).</summary>
        Task<bool> IsSubprojectMemberAsync(int userId, int subprojectId, CancellationToken cancellationToken = default);
    }
}
