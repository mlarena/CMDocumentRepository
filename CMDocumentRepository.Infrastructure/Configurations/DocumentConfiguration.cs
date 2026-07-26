using System.Text.Json;
using CMDocumentRepository.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMDocumentRepository.Infrastructure.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.DocumentNumber).HasMaxLength(50).IsRequired();
        builder.Property(d => d.Title).HasMaxLength(500).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(2000);
        builder.Property(d => d.Version).HasPrecision(4, 1);
        builder.Property(d => d.Status).HasMaxLength(50).IsRequired();
        builder.Property(d => d.FilePath).HasMaxLength(500);
        builder.Property(d => d.FileExtension).HasMaxLength(10);
        builder.Property(d => d.MimeType).HasMaxLength(100);

        builder.HasIndex(d => d.DocumentNumber).IsUnique();
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.CategoryId);
        builder.HasIndex(d => d.DocumentTypeId);
        builder.HasIndex(d => d.CreatedBy);
        builder.HasIndex(d => d.ValidFrom);
        builder.HasIndex(d => d.ValidUntil);

        builder.HasOne(d => d.Category)
            .WithMany()
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.DocumentType)
            .WithMany()
            .HasForeignKey(d => d.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Creator)
            .WithMany()
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(d => d.Metadata)
            .HasColumnType("jsonb");
    }
}
