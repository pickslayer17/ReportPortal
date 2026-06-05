
namespace ReportPortal.DAL.Exceptions
{
    public class EmailAlreadyExistsException : ReportPortalExceptionBase
    {
        public EmailAlreadyExistsException(string message) : base(message) { }
        public EmailAlreadyExistsException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
