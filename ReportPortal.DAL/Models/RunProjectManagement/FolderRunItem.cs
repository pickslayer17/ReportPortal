using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class FolderRunItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public ICollection<int> ChildFolderIds { get; set; }
        public ICollection<int> TestIds { get; set; }
    }
}
