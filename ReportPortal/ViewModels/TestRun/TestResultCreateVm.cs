using ReportPortal.DAL.Enums;

namespace ReportPortal.ViewModels.TestRun
{
    public class TestResultCreateVm
    {
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public byte[] ScreenShot { get; set; }
        public TestOutcome TestOutcome { get; set; }
    }
}
