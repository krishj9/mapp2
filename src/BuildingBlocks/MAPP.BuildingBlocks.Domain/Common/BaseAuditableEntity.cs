namespace MAPP.BuildingBlocks.Domain.Common;

/// <summary>
/// Base auditable entity adapted from Ardalis Clean Architecture template
/// Provides audit trail functionality for entities
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
