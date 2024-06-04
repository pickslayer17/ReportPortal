
namespace ReportPortal.DAL.Exceptions
{
    public class TestWithSuchNameAlreadyExists : ReportPortalExceptionBase
    {
        public TestWithSuchNameAlreadyExists(string message) : base(message)
        {
        }

        public TestWithSuchNameAlreadyExists(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
