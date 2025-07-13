using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.ValueObjects;

/// <summary>
/// Tag value object for child observations
/// </summary>
public class Tag : ValueObject
{
    public string Value { get; private set; } = string.Empty;
    public string NormalizedValue { get; private set; } = string.Empty;

    private Tag() { } // For EF Core

    public Tag(string value)
    {
        Value = Guard.Against.NullOrEmpty(value, nameof(value));
        NormalizedValue = value.ToLowerInvariant().Trim();
    }

    public static Tag Create(string value)
    {
        return new Tag(value);
    }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return NormalizedValue;
    }

    public override string ToString() => Value;

    public static implicit operator string(Tag tag) => tag.Value;
    public static implicit operator Tag(string value) => new(value);
} 