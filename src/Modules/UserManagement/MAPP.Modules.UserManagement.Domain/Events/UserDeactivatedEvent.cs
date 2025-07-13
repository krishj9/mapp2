using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class UserDeactivatedEvent : BaseEvent
{
    public User User { get; }

    public UserDeactivatedEvent(User user)
    {
        User = user;
    }
}
