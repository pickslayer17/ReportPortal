using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class FolderConfiguration : IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder.ToTable("Folders", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar").IsRequired();
            builder.Property(e => e.ParentId).HasColumnName("ParentId").HasColumnType("int");
            builder.HasMany(e => e.Folders).WithOne(e => e.Folder1).HasForeignKey(e => e.ParentId).OnDelete(deleteBehavior: DeleteBehavior.Cascade);

        }
        //public FolderConfiguration()
        //{

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
        //}
    }
}
