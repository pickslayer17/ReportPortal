
namespace ReportPortal.DAL.Exceptions
{
    public class ProjectNotFoundException : ReportPortalExceptionBase
    {
        public ProjectNotFoundException(string message) : base(message)
        {
        }

        public ProjectNotFoundException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
