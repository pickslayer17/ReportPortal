
namespace ReportPortal.DAL.Exceptions
{
    public class FolderNotFoundException : ReportPortalExceptionBase
    {
        public FolderNotFoundException(string message) : base(message)
        {
        }

        public FolderNotFoundException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
