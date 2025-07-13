using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class RoleCreatedEvent : BaseEvent
{
    public Role Role { get; }

    public RoleCreatedEvent(Role role)
    {
        Role = role;
    }
}
