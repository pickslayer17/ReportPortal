
namespace ReportPortal.DAL.Exceptions
{
    public class SubprojectNotFoundException : ReportPortalExceptionBase
    {
        public SubprojectNotFoundException(string message) : base(message) { }
        public SubprojectNotFoundException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
