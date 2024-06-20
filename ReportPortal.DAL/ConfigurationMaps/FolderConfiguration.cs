using ReportPortal.DAL.Models.RunProjectManagement;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class FolderConfiguration : EntityTypeConfiguration<Folder>
    {
        public FolderConfiguration()
        {
            //ToTable("MarketingFolders", "dbo");
            //HasKey(e => e.ID);
            //Property(e => e.ID).HasColumnName("ID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //Property(e => e.ClientID).HasColumnName("ClientID").HasColumnType("int").IsRequired();
            //Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(440);
            //Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar").IsOptional().HasMaxLength(2000);
            //Property(e => e.ParentID).HasColumnName("ParentID").HasColumnType("int").IsOptional();
            //Property(e => e.Created).HasColumnName("Created").HasColumnType("datetime").IsRequired().HasPrecision(3);
            //Property(e => e.CreatedBy).HasColumnName("CreatedBy").HasColumnType("int").IsRequired();
            //Property(e => e.Modified).HasColumnName("Modified").HasColumnType("datetime").IsRequired().HasPrecision(3);
            //Property(e => e.ModifiedBy).HasColumnName("ModifiedBy").HasColumnType("int").IsRequired();
            //Property(e => e.FolderTypeID).HasColumnName("FolderTypeID").HasColumnType("tinyint").IsRequired();

            //HasMany(e => e.MarketingFiles).WithOptional(e => e.MarketingFolder).HasForeignKey(e => e.FolderID).WillCascadeOnDelete(false);
            //HasMany(e => e.MarketingFolders1).WithOptional(e => e.MarketingFolder1).HasForeignKey(e => e.ParentID).WillCascadeOnDelete(false);
            //HasRequired(e => e.Client).WithMany(e => e.MarketingFolders).HasForeignKey(e => e.ClientID).WillCascadeOnDelete(false);
        }
    }
}
