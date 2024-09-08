using ReportPortal.BL.Services;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.BL.Models
{
    public class FolderDto
    {
        public int Id { get; set; }
        public int RunId { get; set; }
        public string Name { get; set; }
        public Folder Parent { get; set; }
        public ICollection<Folder> Children { get; set; }
        public ICollection<TestDto> Tests { get; set; }
    }

}
