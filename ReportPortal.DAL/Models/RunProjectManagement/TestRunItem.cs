using ReportPortal.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class TestRunItem
    {
        [Key]
        public int Id { get; set; }
        public int RunId { get; set; }
        public string Name { get; set; }
        public int FolderId { get; set; }
        public List<int>? TestResultIds { get; set; }
    }
}
