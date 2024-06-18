using ReportPortal.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class TestResult
    {
        [Key]
        public int Id { get; set; }
        public int TestId { get; set; }
        public virtual Test  test { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public byte[] ScreenShot { get; set; }
        public TestOutcome TestOutcome { get; set; }
    }

}
