using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// Progression point entity configuration following clean architecture patterns
/// </summary>
public class ProgressionPointConfiguration : IEntityTypeConfiguration<ProgressionPoint>
{
    public void Configure(EntityTypeBuilder<ProgressionPoint> builder)
    {
        builder.ToTable("ProgressionPoints", "observations");

        builder.Property(p => p.Points)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.Order)
            .HasMaxLength(10);

        builder.Property(p => p.CategoryInformation)
            .HasMaxLength(2000);

        builder.Property(p => p.SortOrder)
            .IsRequired();

        builder.Property(p => p.AttributeId)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure relationships
        builder.HasOne(p => p.Attribute)
            .WithMany(a => a.ProgressionPoints)
            .HasForeignKey(p => p.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(p => p.AttributeId);
        builder.HasIndex(p => p.Points);
        builder.HasIndex(p => p.SortOrder);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => new { p.AttributeId, p.SortOrder });
        builder.HasIndex(p => p.Created);
    }
} 