
namespace ReportPortal.DAL.Exceptions
{
    public class TestNotFoundException : ReportPortalExceptionBase
    {
        public TestNotFoundException(string message) : base(message)
        {
        }

        public TestNotFoundException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
