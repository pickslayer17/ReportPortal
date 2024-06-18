using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Test
    {
        [Key]
        public int Id { get; set; }
        public int RunId { get; set; }
        public string Name { get; set; }
        public int FolderId { get; set; }
        public virtual Folder Folder { get; set; }
    }
}
