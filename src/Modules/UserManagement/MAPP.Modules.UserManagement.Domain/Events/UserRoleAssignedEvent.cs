using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class UserRoleAssignedEvent : BaseEvent
{
    public User User { get; }
    public Role Role { get; }

    public UserRoleAssignedEvent(User user, Role role)
    {
        User = user;
        Role = role;
    }
}
