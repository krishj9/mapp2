using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// Observation entity configuration following clean architecture patterns
/// </summary>
public class ObservationConfiguration : IEntityTypeConfiguration<Observation>
{
    public void Configure(EntityTypeBuilder<Observation> builder)
    {
        builder.Property(o => o.ChildId)
            .IsRequired();

        builder.Property(o => o.ChildName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.TeacherId)
            .IsRequired();

        builder.Property(o => o.TeacherName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.DomainId)
            .IsRequired();

        builder.Property(o => o.DomainName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.AttributeId)
            .IsRequired();

        builder.Property(o => o.AttributeName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.ObservationText)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(o => o.ObservationDate)
            .IsRequired();

        builder.Property(o => o.LearningContext)
            .HasMaxLength(1000);

        builder.Property(o => o.IsDraft)
            .IsRequired();

        // Configure relationships
        builder.HasMany(o => o.MediaItems)
            .WithOne(m => m.Observation)
            .HasForeignKey(m => m.ObservationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure value objects
        builder.OwnsMany(o => o.Tags, tag =>
        {
            tag.WithOwner()
                .HasForeignKey("ObservationId");
            
            tag.Property(t => t.Value)
                .HasMaxLength(100)
                .IsRequired();

            tag.Property(t => t.NormalizedValue)
                .HasMaxLength(100)
                .IsRequired();

            tag.HasIndex(t => t.NormalizedValue);
        });

        // Configure progression point IDs as JSON
        builder.Property(o => o.ProgressionPointIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList()
                    .AsReadOnly())
            .HasColumnType("text");

        // Configure indexes
        builder.HasIndex(o => o.ChildId);
        builder.HasIndex(o => o.TeacherId);
        builder.HasIndex(o => o.DomainId);
        builder.HasIndex(o => o.AttributeId);
        builder.HasIndex(o => o.ObservationDate);
        builder.HasIndex(o => o.IsDraft);
        builder.HasIndex(o => o.Created);
    }
} 