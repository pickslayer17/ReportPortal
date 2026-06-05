using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class SubprojectConfiguration : IEntityTypeConfiguration<Subproject>
    {
        public void Configure(EntityTypeBuilder<Subproject> builder)
        {
            builder.ToTable("Subprojects", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("int").IsRequired();
            builder.HasOne(e => e.Project).WithMany(p => p.Subprojects).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => e.ProjectId);
        }
    }
}
