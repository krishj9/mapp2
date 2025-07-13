using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.ValueObjects;

/// <summary>
/// File size value object for media items
/// </summary>
public class FileSize : ValueObject
{
    public long Bytes { get; private set; }
    public string FormattedSize { get; private set; } = string.Empty;

    private FileSize() { } // For EF Core

    private FileSize(long bytes)
    {
        Bytes = Guard.Against.Negative(bytes, nameof(bytes));
        FormattedSize = FormatBytes(bytes);
    }

    public static FileSize FromBytes(long bytes)
    {
        return new FileSize(bytes);
    }

    public static FileSize FromKilobytes(double kilobytes)
    {
        return new FileSize((long)(kilobytes * 1024));
    }

    public static FileSize FromMegabytes(double megabytes)
    {
        return new FileSize((long)(megabytes * 1024 * 1024));
    }

    public static FileSize FromGigabytes(double gigabytes)
    {
        return new FileSize((long)(gigabytes * 1024 * 1024 * 1024));
    }

    public double ToKilobytes() => Bytes / 1024.0;
    public double ToMegabytes() => Bytes / (1024.0 * 1024.0);
    public double ToGigabytes() => Bytes / (1024.0 * 1024.0 * 1024.0);

    public bool IsEmpty() => Bytes == 0;

    public bool IsLargerThan(FileSize other) => Bytes > other.Bytes;
    public bool IsSmallerThan(FileSize other) => Bytes < other.Bytes;

    private static string FormatBytes(long bytes)
    {
        if (bytes == 0) return "0 B";

        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        int i = 0;
        double dblSByte = bytes;
        while (dblSByte >= 1024 && i < suffixes.Length - 1)
        {
            dblSByte /= 1024;
            i++;
        }

        return $"{dblSByte:0.##} {suffixes[i]}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Bytes;
    }

    public override string ToString() => FormattedSize;

    public static implicit operator long(FileSize fileSize) => fileSize.Bytes;
    public static implicit operator FileSize(long bytes) => new(bytes);

    public static bool operator >(FileSize left, FileSize right) => left.Bytes > right.Bytes;
    public static bool operator <(FileSize left, FileSize right) => left.Bytes < right.Bytes;
    public static bool operator >=(FileSize left, FileSize right) => left.Bytes >= right.Bytes;
    public static bool operator <=(FileSize left, FileSize right) => left.Bytes <= right.Bytes;
} 