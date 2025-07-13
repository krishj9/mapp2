using MAPP.BuildingBlocks.Domain.Common;

namespace MAPP.Modules.UserManagement.Domain.Entities;

/// <summary>
/// User role junction entity following DDD patterns from Ardalis template
/// </summary>
public class UserRole : BaseAuditableEntity
{
    public int UserId { get; private set; }
    public int RoleId { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    // Private constructor for EF Core
    private UserRole() { }

    public UserRole(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
