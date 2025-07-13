using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.ValueObjects;

/// <summary>
/// File path value object for media items
/// </summary>
public class FilePath : ValueObject
{
    public string Value { get; private set; } = string.Empty;
    public string NormalizedValue { get; private set; } = string.Empty;

    private FilePath() { } // For EF Core

    public FilePath(string value)
    {
        Value = Guard.Against.NullOrEmpty(value, nameof(value));
        NormalizedValue = NormalizePath(value);
    }

    public static FilePath Create(string value)
    {
        return new FilePath(value);
    }

    public string GetFileName()
    {
        return Path.GetFileName(Value);
    }

    public string GetFileNameWithoutExtension()
    {
        return Path.GetFileNameWithoutExtension(Value);
    }

    public string GetExtension()
    {
        return Path.GetExtension(Value);
    }

    public string GetDirectoryName()
    {
        return Path.GetDirectoryName(Value) ?? string.Empty;
    }

    public FilePath Combine(string relativePath)
    {
        var combinedPath = Path.Combine(Value, relativePath);
        return new FilePath(combinedPath);
    }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Value);
    }

    public bool HasExtension()
    {
        return !string.IsNullOrEmpty(GetExtension());
    }

    public bool IsAbsolutePath()
    {
        return Path.IsPathRooted(Value);
    }

    public bool IsRelativePath()
    {
        return !IsAbsolutePath();
    }

    private static string NormalizePath(string path)
    {
        // Normalize path separators to forward slashes (cloud storage standard)
        return path.Replace('\\', '/').Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return NormalizedValue;
    }

    public override string ToString() => Value;

    public static implicit operator string(FilePath filePath) => filePath.Value;
    public static implicit operator FilePath(string value) => new(value);
} 