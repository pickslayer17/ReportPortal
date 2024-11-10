using ReportPortal.DAL.Enums;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class TestResult
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public virtual Test Test { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public byte[] ScreenShot { get; set; }
        public TestOutcome TestOutcome { get; set; }
    }

}
