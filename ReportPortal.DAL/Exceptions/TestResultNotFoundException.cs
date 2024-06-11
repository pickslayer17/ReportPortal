namespace ReportPortal.DAL.Exceptions
{
    public class TestResultNotFoundException : ReportPortalExceptionBase
    {
        public TestResultNotFoundException(string message) : base(message)
        {
        }

        public TestResultNotFoundException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
