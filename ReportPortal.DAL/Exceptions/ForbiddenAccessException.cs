
namespace ReportPortal.DAL.Exceptions
{
    /// <summary>
    /// Thrown when an authenticated user tries to access a resource that belongs to a project
    /// they are not a member of (tenant scope violation).
    /// </summary>
    public class ForbiddenAccessException : ReportPortalExceptionBase
    {
        public ForbiddenAccessException(string message) : base(message) { }
        public ForbiddenAccessException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
