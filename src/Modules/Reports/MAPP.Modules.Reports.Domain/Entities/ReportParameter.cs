using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.Reports.Domain.Entities;

/// <summary>
/// Report parameter entity following DDD patterns from Ardalis template
/// </summary>
public class ReportParameter : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string DataType { get; private set; } = string.Empty;
    public bool IsRequired { get; private set; }
    public string? DefaultValue { get; private set; }
    public string? Description { get; private set; }
    public int ReportId { get; private set; }

    // Navigation property
    public Report Report { get; private set; } = null!;

    // Private constructor for EF Core
    private ReportParameter() { }

    public ReportParameter(string name, string dataType, bool isRequired, string? defaultValue, int reportId)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        DataType = Guard.Against.NullOrEmpty(dataType, nameof(dataType));
        IsRequired = isRequired;
        DefaultValue = defaultValue;
        ReportId = reportId;
    }

    public void UpdateDetails(string name, string dataType, bool isRequired, string? defaultValue = null, string? description = null)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        DataType = Guard.Against.NullOrEmpty(dataType, nameof(dataType));
        IsRequired = isRequired;
        DefaultValue = defaultValue;
        Description = description;
    }
}
