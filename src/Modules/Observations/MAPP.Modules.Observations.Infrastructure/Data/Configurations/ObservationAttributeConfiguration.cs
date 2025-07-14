using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// ObservationAttribute entity configuration following Ardalis patterns
/// </summary>
public class ObservationAttributeConfiguration : IEntityTypeConfiguration<ObservationAttribute>
{
    public void Configure(EntityTypeBuilder<ObservationAttribute> builder)
    {
        builder.ToTable("ObservationAttributes", "observations");

        builder.Property(a => a.Number)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.CategoryInformation)
            .HasMaxLength(2000);

        builder.Property(a => a.SortOrder)
            .IsRequired();

        builder.Property(a => a.DomainId)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure relationships
        builder.HasOne(a => a.Domain)
            .WithMany(d => d.Attributes)
            .HasForeignKey(a => a.DomainId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.ProgressionPoints)
            .WithOne(p => p.Attribute)
            .HasForeignKey(p => p.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(a => a.DomainId);
        builder.HasIndex(a => a.Number);
        builder.HasIndex(a => a.SortOrder);
        builder.HasIndex(a => a.IsActive);
        builder.HasIndex(a => new { a.DomainId, a.Number }).IsUnique();
    }
}
