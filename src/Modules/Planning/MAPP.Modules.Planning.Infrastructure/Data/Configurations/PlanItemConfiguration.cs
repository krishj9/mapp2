using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Planning.Domain.Entities;

namespace MAPP.Modules.Planning.Infrastructure.Data.Configurations;

/// <summary>
/// Plan item entity configuration following Ardalis patterns
/// </summary>
public class PlanItemConfiguration : IEntityTypeConfiguration<PlanItem>
{
    public void Configure(EntityTypeBuilder<PlanItem> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.AssignedTo)
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

        // Configure indexes
        builder.HasIndex(p => p.PlanId);
        builder.HasIndex(p => p.AssignedTo);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.DueDate);
    }
}
