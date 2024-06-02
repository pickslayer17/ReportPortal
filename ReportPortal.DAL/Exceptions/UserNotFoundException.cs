
namespace ReportPortal.DAL.Exceptions
{
    public class UserNotFoundException : ReportPortalExceptionBase
    {
        public UserNotFoundException(string message) : base(message) { }
        public UserNotFoundException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
