using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data.Configurations;

/// <summary>
/// Observation artifact entity configuration following clean architecture patterns
/// </summary>
public class ObservationArtifactConfiguration : IEntityTypeConfiguration<ObservationArtifact>
{
    public void Configure(EntityTypeBuilder<ObservationArtifact> builder)
    {
        builder.Property(a => a.ObservationId)
            .IsRequired();

        builder.Property(a => a.OriginalFileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.StoredFileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.BucketName)
            .HasMaxLength(200);

        builder.Property(a => a.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.PublicUrl)
            .HasMaxLength(1000);

        builder.Property(a => a.Caption)
            .HasMaxLength(1000);

        builder.Property(a => a.DisplayOrder)
            .IsRequired();

        builder.Property(a => a.IsUploaded)
            .IsRequired();

        builder.Property(a => a.UploadError)
            .HasMaxLength(2000);

        builder.Property(a => a.UploadedBy)
            .HasMaxLength(200);

        builder.Property(a => a.Metadata)
            .HasColumnType("jsonb");

        // Configure value objects
        builder.OwnsOne(a => a.StoragePath, path =>
        {
            path.Property(p => p.Value)
                .HasColumnName("StoragePath")
                .HasMaxLength(1000)
                .IsRequired();

            path.Property(p => p.NormalizedValue)
                .HasColumnName("StoragePathNormalized")
                .HasMaxLength(1000)
                .IsRequired();
        });

        builder.OwnsOne(a => a.FileSizeBytes, size =>
        {
            size.Property(s => s.Bytes)
                .HasColumnName("FileSizeBytes")
                .IsRequired();

            size.Property(s => s.FormattedSize)
                .HasColumnName("FileSizeFormatted")
                .HasMaxLength(50)
                .IsRequired();
        });

        // Configure MediaType enum
        builder.Property(a => a.MediaType)
            .HasConversion<int>();

        // Configure indexes
        builder.HasIndex(a => a.ObservationId);
        builder.HasIndex(a => a.MediaType);
        builder.HasIndex(a => a.IsUploaded);
        builder.HasIndex(a => a.DisplayOrder);
        builder.HasIndex(a => a.Created);
    }
} 