using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Planning.Domain.Entities;
using MAPP.Modules.Planning.Domain.ValueObjects;

namespace MAPP.Modules.Planning.Infrastructure.Data.Configurations;

/// <summary>
/// Plan entity configuration following clean architecture patterns
/// </summary>
public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.OwnerId)
            .HasMaxLength(450); // Standard ASP.NET Identity user ID length

        // Configure Priority value object
        builder.OwnsOne(p => p.Priority, priority =>
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
        builder.HasMany(p => p.Items)
            .WithOne(i => i.Plan)
            .HasForeignKey(i => i.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.Created);
    }
}
