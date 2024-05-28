using ReportPortal.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class TestRunItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public ICollection<int> TestResultIds { get; set; }
    }
}
