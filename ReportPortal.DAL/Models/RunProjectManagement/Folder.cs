using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Folder : RootFolder
    {
        public virtual ICollection<Test> Tests { get; set; }
    }
}
