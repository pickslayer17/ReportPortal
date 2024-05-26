using ReportPortal.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models
{
    public abstract class RunItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public RunItemType Type { get; set; }
        public int ParentId { get; set; }
        public int[] ChildrenIds { get; set; }
    }
}
