
namespace ReportPortal.DAL.Exceptions
{
    public class TestNotFoundExeption : ReportPortalExceptionBase
    {
        public TestNotFoundExeption(string message) : base(message)
        {
        }

        public TestNotFoundExeption(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
