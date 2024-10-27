using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class TestResultConfiguration : IEntityTypeConfiguration<TestResult>
    {
        public void Configure(EntityTypeBuilder<TestResult> builder)
        {
            builder.ToTable("TestResults", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.TestId).HasColumnName("TestId").HasColumnType("int").IsRequired();
            builder.Property(e => e.TestOutcome).HasColumnName("TestOutcome").HasColumnType("int").IsRequired();
            builder.Property(e => e.ErrorMessage).HasColumnName("ErrorMessage").HasColumnType("nvarchar(MAX)");
            builder.Property(e => e.StackTrace).HasColumnName("StackTrace").HasColumnType("nvarchar(MAX)");
            builder.Property(e => e.ScreenShot).HasColumnName("ScreenShot").HasColumnType("varbinary(max)");
            builder.HasOne(e => e.Test).WithMany(e => e.TestResults).HasForeignKey(e => e.TestId).OnDelete(deleteBehavior: DeleteBehavior.Cascade);
        }
    }
}
