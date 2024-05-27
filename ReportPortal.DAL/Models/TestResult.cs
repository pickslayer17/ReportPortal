using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models
{
    public class TestResult
    {
        [Key]
        public int Id { get; set; }
        public int TestId { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public byte[] ScreenShot { get; set; }
    }

}
