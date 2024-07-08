using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.ToTable("Tests", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(MAX)").IsRequired();
            builder.Property(e => e.RunId).HasColumnName("RunId").HasColumnType("int");
            builder.Property(e => e.FolderId).HasColumnName("FolderId").HasColumnType("int");
            builder.HasOne(e => e.Folder).WithMany(e => e.Tests).HasForeignKey(e => e.FolderId).OnDelete(deleteBehavior: DeleteBehavior.ClientCascade);
        }
    }
}
