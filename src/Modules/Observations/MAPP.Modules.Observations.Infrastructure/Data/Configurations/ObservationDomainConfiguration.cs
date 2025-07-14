using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// ObservationDomain entity configuration following Ardalis patterns
/// </summary>
public class ObservationDomainConfiguration : IEntityTypeConfiguration<ObservationDomain>
{
    public void Configure(EntityTypeBuilder<ObservationDomain> builder)
    {
        builder.ToTable("ObservationDomains", "observations");

        builder.Property(d => d.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.CategoryName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.CategoryTitle)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.SortOrder)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure relationships
        builder.HasMany(d => d.Attributes)
            .WithOne(a => a.Domain)
            .HasForeignKey(a => a.DomainId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(d => d.SortOrder);
        builder.HasIndex(d => d.IsActive);
        builder.HasIndex(d => d.Name);
    }
}
