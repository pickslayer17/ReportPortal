namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Run
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SubprojectId { get; set; }
        public virtual Subproject Subproject { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
    }
}
