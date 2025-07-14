using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Observations.Domain.ValueObjects;

/// <summary>
/// Tag value object for child observations that provides flexible labeling and categorization beyond formal classification.
///
/// Tags allow teachers to add custom, flexible labels to observations that complement the structured
/// domain/attribute/progression point classification system. This enables personalized categorization,
/// improved searchability, and trend analysis across observations.
///
/// Examples of common tag usage:
/// - Behavioral: "independent-play", "needs-support", "breakthrough-moment", "challenging-behavior"
/// - Activity: "outdoor-play", "art-activity", "circle-time", "free-choice"
/// - Learning Style: "visual-learner", "hands-on", "collaborative", "quiet-observer"
/// - Assessment: "portfolio-worthy", "parent-conference", "iep-relevant", "milestone"
///
/// Tags are implemented as value objects (compared by value, not identity) and are case-insensitive
/// for consistent searching and matching. Each tag is owned by a specific observation and stored
/// in a separate database table with indexing on the normalized value for efficient querying.
/// </summary>
public class Tag : ValueObject
{
    /// <summary>
    /// The original tag text as entered by the teacher, preserving case and formatting.
    /// Maximum length: 100 characters.
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    /// The normalized (lowercase, trimmed) version of the tag value used for case-insensitive
    /// searching, matching, and database indexing. This ensures consistent behavior when
    /// comparing tags regardless of how they were originally entered.
    /// </summary>
    public string NormalizedValue { get; private set; } = string.Empty;

    /// <summary>
    /// Private parameterless constructor required by Entity Framework Core for materialization.
    /// </summary>
    private Tag() { }

    /// <summary>
    /// Creates a new tag with the specified value.
    /// </summary>
    /// <param name="value">The tag text. Cannot be null or empty. Will be trimmed and normalized for consistent comparison.</param>
    /// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
    public Tag(string value)
    {
        Value = Guard.Against.NullOrEmpty(value, nameof(value));
        NormalizedValue = value.ToLowerInvariant().Trim();
    }

    /// <summary>
    /// Factory method to create a new tag with the specified value.
    /// Provides a more explicit way to create tags compared to using the constructor directly.
    /// </summary>
    /// <param name="value">The tag text. Cannot be null or empty.</param>
    /// <returns>A new Tag instance with the specified value.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
    public static Tag Create(string value)
    {
        return new Tag(value);
    }

    /// <summary>
    /// Determines whether this tag is empty (null, empty, or whitespace only).
    /// </summary>
    /// <returns>True if the tag value is null, empty, or contains only whitespace; otherwise false.</returns>
    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Value);
    }

    /// <summary>
    /// Gets the components used for equality comparison. Tags are considered equal if their
    /// normalized values are the same, ensuring case-insensitive equality.
    /// </summary>
    /// <returns>An enumerable of objects used for equality comparison.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return NormalizedValue;
    }

    /// <summary>
    /// Returns the original tag value as a string.
    /// </summary>
    /// <returns>The original tag text preserving case and formatting.</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a Tag to its string value.
    /// </summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>The tag's original value as a string.</returns>
    public static implicit operator string(Tag tag) => tag.Value;

    /// <summary>
    /// Implicitly converts a string to a Tag.
    /// </summary>
    /// <param name="value">The string value to convert to a tag.</param>
    /// <returns>A new Tag instance with the specified value.</returns>
    public static implicit operator Tag(string value) => new(value);
} 