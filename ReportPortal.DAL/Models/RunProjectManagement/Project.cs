using ReportPortal.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<int> RunIds { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
    }
}
