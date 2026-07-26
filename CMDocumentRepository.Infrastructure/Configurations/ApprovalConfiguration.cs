using CMDocumentRepository.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMDocumentRepository.Infrastructure.Configurations;

public class ApprovalConfiguration : IEntityTypeConfiguration<Approval>
{
    public void Configure(EntityTypeBuilder<Approval> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Status).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Comment).HasMaxLength(1000);

        builder.HasOne(a => a.Document)
            .WithMany(d => d.Approvals)
            .HasForeignKey(a => a.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Approver)
            .WithMany()
            .HasForeignKey(a => a.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.DocumentId, a.ApproverId }).IsUnique();
    }
}
