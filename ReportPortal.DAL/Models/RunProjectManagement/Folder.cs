using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Folder : AbstractFolder
    {
        public virtual ICollection<Test> Tests { get; set; }
    }
}
