using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// Observation data entity configuration following Ardalis patterns
/// </summary>
public class ObservationDataConfiguration : IEntityTypeConfiguration<ObservationData>
{
    public void Configure(EntityTypeBuilder<ObservationData> builder)
    {
        builder.Property(t => t.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Value)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.Unit)
            .HasMaxLength(50);

        // Configure indexes
        builder.HasIndex(d => d.ObservationId);
        builder.HasIndex(d => d.Key);
        builder.HasIndex(d => new { d.ObservationId, d.Key });
    }
}
