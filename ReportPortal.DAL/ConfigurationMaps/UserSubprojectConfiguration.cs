using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class UserSubprojectConfiguration : IEntityTypeConfiguration<UserSubproject>
    {
        public void Configure(EntityTypeBuilder<UserSubproject> builder)
        {
            builder.ToTable("UserSubprojects");
            builder.HasKey(e => new { e.UserId, e.SubprojectId });
            builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnType("int").IsRequired();
            builder.Property(e => e.SubprojectId).HasColumnName("SubprojectId").HasColumnType("int").IsRequired();
            builder.HasOne(e => e.User).WithMany(u => u.UserSubprojects).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Subproject).WithMany(s => s.UserSubprojects).HasForeignKey(e => e.SubprojectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => e.SubprojectId);
        }
    }
}
