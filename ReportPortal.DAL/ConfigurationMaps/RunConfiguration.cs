using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class RunConfiguration : IEntityTypeConfiguration<Run>
    {
        public void Configure(EntityTypeBuilder<Run> builder)
        {
            builder.ToTable("Runs", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(MAX)").IsRequired();
            builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("int");
            builder.HasOne(e => e.Project).WithMany(e => e.Runs).HasForeignKey(e => e.ProjectId).OnDelete(deleteBehavior: DeleteBehavior.ClientCascade);
            builder.Property(e => e.RootFolderId).HasColumnName("RootFolderId").HasColumnType("int");
            //builder.HasOne(e => e.RootFolder).WithOne(e => e.Run).HasForeignKey<RootFolder>(e => e.RunId).OnDelete(DeleteBehavior.ClientCascade);
            //builder.HasMany(e => e.Folders).WithOne(e => e.Run).HasForeignKey(e => e.RunId).OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
