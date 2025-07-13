using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// Observation entity configuration following Ardalis patterns
/// </summary>
public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.ObserverId)
            .HasMaxLength(450); // Standard ASP.NET Identity user ID length

        builder.Property(t => t.Location)
            .HasMaxLength(500);

        // Configure Priority value object
        builder.OwnsOne(o => o.Priority, priority =>
        {
            priority.Property(p => p.Value)
                .HasColumnName("Priority")
                .IsRequired();

            priority.Property(p => p.Name)
                .HasColumnName("PriorityName")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Configure Status enum
        builder.Property(t => t.Status)
            .HasConversion<int>();

        // Configure relationships
        builder.HasMany(o => o.Data)
            .WithOne(d => d.Observation)
            .HasForeignKey(d => d.ObservationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(o => o.ObserverId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.ObservedAt);
        builder.HasIndex(o => o.Location);
        builder.HasIndex(o => o.Created);
    }
}
