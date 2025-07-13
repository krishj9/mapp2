using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.UserManagement.Domain.Events;

namespace MAPP.Modules.UserManagement.Domain.Entities;

/// <summary>
/// Role entity following DDD patterns from Ardalis template
/// </summary>
public class Role : BaseAuditableEntity
{
    private readonly List<Permission> _permissions = new();

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }

    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    // Private constructor for EF Core
    private Role() { }

    public Role(string name, string? description = null, bool isSystemRole = false)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description;
        IsSystemRole = isSystemRole;

        AddDomainEvent(new RoleCreatedEvent(this));
    }

    public void UpdateDetails(string name, string? description = null)
    {
        if (IsSystemRole)
        {
            throw new InvalidOperationException("Cannot modify system roles");
        }

        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description;
    }

    public void AddPermission(Permission permission)
    {
        if (_permissions.Any(p => p.Id == permission.Id))
        {
            throw new InvalidOperationException($"Role already has permission: {permission.Name}");
        }

        _permissions.Add(permission);
        AddDomainEvent(new RolePermissionAddedEvent(this, permission));
    }

    public void RemovePermission(Permission permission)
    {
        if (IsSystemRole)
        {
            throw new InvalidOperationException("Cannot modify permissions for system roles");
        }

        var existingPermission = _permissions.FirstOrDefault(p => p.Id == permission.Id);
        if (existingPermission == null)
        {
            throw new InvalidOperationException($"Role does not have permission: {permission.Name}");
        }

        _permissions.Remove(existingPermission);
        AddDomainEvent(new RolePermissionRemovedEvent(this, permission));
    }
}
