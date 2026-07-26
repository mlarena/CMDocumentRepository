using CMDocumentRepository.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMDocumentRepository.Infrastructure.Configurations;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.HasKey(dv => dv.Id);
        builder.Property(dv => dv.VersionNumber).HasPrecision(4, 1);
        builder.Property(dv => dv.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(dv => dv.ChangeComment).HasMaxLength(1000);

        builder.HasIndex(dv => new { dv.DocumentId, dv.VersionNumber }).IsUnique();

        builder.HasOne(dv => dv.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(dv => dv.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dv => dv.Creator)
            .WithMany()
            .HasForeignKey(dv => dv.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
