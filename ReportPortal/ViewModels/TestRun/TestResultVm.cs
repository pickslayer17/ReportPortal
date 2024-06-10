using ReportPortal.DAL.Enums;

namespace ReportPortal.ViewModels.TestRun
{
    public class TestResultVm
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int RunId { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public byte[] ScreenShot { get; set; }
        public TestOutcome TestOutcome { get; set; }
    }

}
