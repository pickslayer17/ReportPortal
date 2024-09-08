namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class AbstractFolder
    {
        public Nullable<int> ParentId { get; set; }
        public virtual AbstractFolder Parent { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Folder> Children { get; set; }
        public int? RunId { get; set; }
        public virtual Run Run { get; set; }
    }
}
