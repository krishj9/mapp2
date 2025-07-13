using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// Progression point entity configuration following Ardalis patterns
/// </summary>
public class ProgressionPointConfiguration : IEntityTypeConfiguration<ProgressionPoint>
{
    public void Configure(EntityTypeBuilder<ProgressionPoint> builder)
    {
        builder.Property(p => p.Points)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(p => p.DomainId)
            .IsRequired();

        builder.Property(p => p.AttributeId)
            .IsRequired();

        builder.Property(p => p.ParentProgressionPointId);

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        // Configure self-referencing relationship
        builder.HasOne(p => p.ParentProgressionPoint)
            .WithMany(p => p.ChildProgressionPoints)
            .HasForeignKey(p => p.ParentProgressionPointId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes
        builder.HasIndex(p => p.DomainId);
        builder.HasIndex(p => p.AttributeId);
        builder.HasIndex(p => p.ParentProgressionPointId);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.DisplayOrder);
        builder.HasIndex(p => new { p.DomainId, p.AttributeId });
        builder.HasIndex(p => p.Created);
    }
} 