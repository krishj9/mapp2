using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class UserActivatedEvent : BaseEvent
{
    public User User { get; }

    public UserActivatedEvent(User user)
    {
        User = user;
    }
}
