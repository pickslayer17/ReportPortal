using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class RootFolderConfiguration : IEntityTypeConfiguration<RootFolder>
    {
        public void Configure(EntityTypeBuilder<RootFolder> builder)
        {
            builder.ToTable("Folders", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(MAX)").IsRequired();
            builder.Property(e => e.RunId).HasColumnName("ParentId").HasColumnType("int");
            builder.HasOne(e => e.Run).WithOne(e => e.RootFolder).HasForeignKey<Run>(e => e.RootFolderId).OnDelete(deleteBehavior: DeleteBehavior.ClientCascade);
        }
    }
}
