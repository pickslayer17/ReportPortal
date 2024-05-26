using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Run> Runs { get; set; }
    }
}
