using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class RolePermissionRemovedEvent : BaseEvent
{
    public Role Role { get; }
    public Permission Permission { get; }

    public RolePermissionRemovedEvent(Role role, Permission permission)
    {
        Role = role;
        Permission = permission;
    }
}
