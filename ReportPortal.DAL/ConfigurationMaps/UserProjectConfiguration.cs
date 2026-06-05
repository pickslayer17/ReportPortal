using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class UserProjectConfiguration : IEntityTypeConfiguration<UserProject>
    {
        public void Configure(EntityTypeBuilder<UserProject> builder)
        {
            builder.ToTable("UserProjects");
            builder.HasKey(e => new { e.UserId, e.ProjectId });
            builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnType("int").IsRequired();
            builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("int").IsRequired();
            builder.HasOne(e => e.User).WithMany(u => u.UserProjects).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.Project).WithMany(p => p.UserProjects).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => e.ProjectId);
        }
    }
}
