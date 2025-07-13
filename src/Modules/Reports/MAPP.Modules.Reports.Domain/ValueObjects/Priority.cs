using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Reports.Domain.ValueObjects;

/// <summary>
/// Priority value object following DDD patterns from Ardalis template
/// </summary>
public class Priority : ValueObject
{
    public static Priority Low => new(1, "Low");
    public static Priority Medium => new(2, "Medium");
    public static Priority High => new(3, "High");
    public static Priority Critical => new(4, "Critical");

    public int Value { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Priority() { } // For EF Core

    private Priority(int value, string name)
    {
        Value = value;
        Name = name;
    }

    public static Priority FromValue(int value)
    {
        return value switch
        {
            1 => Low,
            2 => Medium,
            3 => High,
            4 => Critical,
            _ => throw new ArgumentException($"Invalid priority value: {value}")
        };
    }

    public static Priority FromName(string name)
    {
        return name.ToLowerInvariant() switch
        {
            "low" => Low,
            "medium" => Medium,
            "high" => High,
            "critical" => Critical,
            _ => throw new ArgumentException($"Invalid priority name: {name}")
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Name;
    }

    public override string ToString() => Name;

    public static implicit operator string(Priority priority) => priority.Name;
    public static implicit operator int(Priority priority) => priority.Value;
}
