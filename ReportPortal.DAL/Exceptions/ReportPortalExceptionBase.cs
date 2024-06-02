using System.Runtime.InteropServices;

namespace ReportPortal.DAL.Exceptions
{
    public abstract class ReportPortalExceptionBase : Exception
    {
        public ReportPortalExceptionBase(string message) : base(message) { }
        public ReportPortalExceptionBase(string message, Exception ex) : base ("", ex)
        {
        }
    }
}
