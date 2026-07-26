using CMDocumentRepository.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMDocumentRepository.Infrastructure.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.HasKey(ss => ss.Id);
        builder.Property(ss => ss.Key).HasMaxLength(100).IsRequired();
        builder.Property(ss => ss.Value).HasMaxLength(2000);
        builder.Property(ss => ss.Description).HasMaxLength(500);

        builder.HasIndex(ss => ss.Key).IsUnique();
    }
}
