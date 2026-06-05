using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.Email).HasColumnName("Email").HasColumnType("nvarchar(256)").IsRequired();
            builder.Property(e => e.Password).HasColumnName("Password").HasColumnType("nvarchar(512)").IsRequired();
            builder.Property(e => e.UserRole).HasColumnName("UserRole").HasColumnType("int").IsRequired();
            builder.HasIndex(e => e.Email).IsUnique();
        }
    }
}
