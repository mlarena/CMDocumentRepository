using CMDocumentRepository.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMDocumentRepository.Infrastructure.Configurations;

public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
{
    public void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder.HasKey(dt => dt.Id);
        builder.Property(dt => dt.Name).HasMaxLength(100).IsRequired();
        builder.Property(dt => dt.Code).HasMaxLength(50).IsRequired();
        builder.Property(dt => dt.Description).HasMaxLength(500);

        builder.HasIndex(dt => dt.Code).IsUnique();
    }
}
