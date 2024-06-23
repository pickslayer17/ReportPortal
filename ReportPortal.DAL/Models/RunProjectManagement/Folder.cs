using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentId { get; set; }
        public virtual Folder Parent { get; set; }
        public virtual ICollection<Folder> Children { get; set; }
    }
}
