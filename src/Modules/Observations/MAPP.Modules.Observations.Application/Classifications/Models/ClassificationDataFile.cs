namespace MAPP.Modules.Observations.Application.Classifications.Models;

/// <summary>
/// Model representing the hierarchical classification data file structure
/// </summary>
public class ClassificationDataFile
{
    /// <summary>
    /// File format version for compatibility tracking
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Timestamp when the data was last updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Source information for audit trail
    /// </summary>
    public string Source { get; set; } = "MAPP Database Export";

    /// <summary>
    /// Complete domains hierarchy with attributes and progression points
    /// </summary>
    public List<DomainFileModel> Domains { get; set; } = new();
}

/// <summary>
/// Domain model for file export/import
/// </summary>
public class DomainFileModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryTitle { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<AttributeFileModel> Attributes { get; set; } = new();
}

/// <summary>
/// Attribute model for file export/import
/// </summary>
public class AttributeFileModel
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CategoryInformation { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ProgressionPointFileModel> ProgressionPoints { get; set; } = new();
}

/// <summary>
/// Progression point model for file export/import
/// </summary>
public class ProgressionPointFileModel
{
    public int Id { get; set; }
    public int Points { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Order { get; set; } = string.Empty;
    public string? CategoryInformation { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
