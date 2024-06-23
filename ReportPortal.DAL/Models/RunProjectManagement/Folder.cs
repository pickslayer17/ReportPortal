﻿using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentId { get; set; }
        public virtual Folder Parent { get; set; }
        public virtual Folder Folder1 { get; set; }
        public virtual ICollection<Folder> Folders { get; set; }
    }
}
