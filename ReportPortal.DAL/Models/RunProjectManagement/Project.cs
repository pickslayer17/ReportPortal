using ReportPortal.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Run> Runs { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
    }
}
