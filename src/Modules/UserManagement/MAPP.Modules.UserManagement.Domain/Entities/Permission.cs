using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.UserManagement.Domain.Entities;

/// <summary>
/// Permission entity following DDD patterns from Ardalis template
/// </summary>
public class Permission : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Resource { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;

    // Private constructor for EF Core
    private Permission() { }

    public Permission(string name, string resource, string action, string? description = null)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Resource = Guard.Against.NullOrEmpty(resource, nameof(resource));
        Action = Guard.Against.NullOrEmpty(action, nameof(action));
        Description = description;
    }

    public void UpdateDetails(string name, string? description = null)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description;
    }
}
