using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(MAX)").IsRequired();
            builder.Property(e => e.ProjectStatus).HasColumnName("ProjectStatus").HasColumnType("int");
        }
    }
}
