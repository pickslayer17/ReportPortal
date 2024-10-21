using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.ConfigurationMaps
{
    public class TestReviewConfiguration : IEntityTypeConfiguration<TestReview>
    {
        public void Configure(EntityTypeBuilder<TestReview> builder)
        {
            builder.ToTable("TestReviews", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("int").IsRequired().UseIdentityColumn();
            builder.Property(e => e.TestId).HasColumnName("TestId").HasColumnType("int").IsRequired();
            builder.Property(e => e.ReviewerId).HasColumnName("ReviewerId").HasColumnType("int");
            builder.Property(e => e.Comments).HasColumnName("Comments").HasColumnType("nvarchar(MAX)");
            builder.Property(e => e.TestReviewOutcome).HasColumnName("TestReviewOutcome").HasColumnType("int");
            builder.HasOne(e => e.Test).WithOne(e => e.TestReview).HasForeignKey<TestReview>(e => e.TestId).OnDelete(deleteBehavior: DeleteBehavior.ClientCascade);
        }
    }
}
