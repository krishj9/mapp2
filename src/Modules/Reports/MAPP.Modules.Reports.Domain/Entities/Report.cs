using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.Reports.Domain.Events;
using MAPP.Modules.Reports.Domain.ValueObjects;
using MAPP.Modules.Reports.Domain.Enums;

namespace MAPP.Modules.Reports.Domain.Entities;

/// <summary>
/// Report aggregate root following DDD patterns from Ardalis template
/// </summary>
public class Report : BaseAuditableEntity
{
    private readonly List<ReportParameter> _parameters = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ReportType Type { get; private set; } = ReportType.Standard;
    public ReportStatus Status { get; private set; } = ReportStatus.Draft;
    public Priority Priority { get; private set; } = Priority.Medium;
    public string? Query { get; private set; }
    public string? Template { get; private set; }
    public string? OutputPath { get; private set; }
    public DateTimeOffset? LastGeneratedAt { get; private set; }
    public string? GeneratedBy { get; private set; }

    public IReadOnlyCollection<ReportParameter> Parameters => _parameters.AsReadOnly();

    // Private constructor for EF Core
    private Report() { }

    public Report(string name, ReportType type, string? description = null)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Type = type;
        Description = description;
        Status = ReportStatus.Draft;
        Priority = Priority.Medium;

        AddDomainEvent(new ReportCreatedEvent(this));
    }

    public void UpdateDetails(string name, string? description = null)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description;
    }

    public void SetPriority(Priority priority)
    {
        Priority = priority;
    }

    public void SetQuery(string query)
    {
        Query = Guard.Against.NullOrEmpty(query, nameof(query));
    }

    public void SetTemplate(string template)
    {
        Template = Guard.Against.NullOrEmpty(template, nameof(template));
    }

    public void Publish()
    {
        if (Status != ReportStatus.Draft)
        {
            throw new InvalidOperationException($"Cannot publish report in {Status} status");
        }

        if (string.IsNullOrEmpty(Query))
        {
            throw new InvalidOperationException("Cannot publish report without a query");
        }

        Status = ReportStatus.Published;
        AddDomainEvent(new ReportPublishedEvent(this));
    }

    public void Archive()
    {
        if (Status == ReportStatus.Archived)
        {
            throw new InvalidOperationException("Report is already archived");
        }

        Status = ReportStatus.Archived;
        AddDomainEvent(new ReportArchivedEvent(this));
    }

    public void RecordGeneration(string generatedBy, string? outputPath = null)
    {
        LastGeneratedAt = DateTimeOffset.UtcNow;
        GeneratedBy = generatedBy;
        OutputPath = outputPath;

        AddDomainEvent(new ReportGeneratedEvent(this));
    }

    public ReportParameter AddParameter(string name, string dataType, bool isRequired = false, string? defaultValue = null)
    {
        var parameter = new ReportParameter(name, dataType, isRequired, defaultValue, Id);
        _parameters.Add(parameter);
        return parameter;
    }

    public void RemoveParameter(ReportParameter parameter)
    {
        _parameters.Remove(parameter);
    }
}
