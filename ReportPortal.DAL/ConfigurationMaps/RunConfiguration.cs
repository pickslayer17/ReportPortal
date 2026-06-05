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
            builder.Property(e => e.SubprojectId).HasColumnName("SubprojectId").HasColumnType("int");
            builder.HasOne(e => e.Subproject).WithMany(e => e.Runs).HasForeignKey(e => e.SubprojectId).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
            builder.HasIndex(e => e.SubprojectId);
        }
    }
}
