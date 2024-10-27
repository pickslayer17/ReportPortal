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
            builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(MAX)").IsRequired();
            builder.Property(e => e.ParentId).HasColumnName("ParentId").HasColumnType("int");
            builder.Property(e => e.FolderLevel).HasColumnName("FolderLevel").HasColumnType("int");
            builder.HasOne(e => e.Parent).WithMany(e => e.Children).HasForeignKey(e => e.ParentId).OnDelete(deleteBehavior: DeleteBehavior.Restrict);
            builder.Property(e => e.RunId).HasColumnName("RunId").HasColumnType("int");
            builder.HasOne(e => e.Run).WithMany(e => e.Folders).HasForeignKey(e => e.RunId).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        }
    }
}
