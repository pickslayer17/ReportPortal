namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Run
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public virtual RootFolder RootFolder { get; set; }
        public int RootFolderId { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
    }
}
