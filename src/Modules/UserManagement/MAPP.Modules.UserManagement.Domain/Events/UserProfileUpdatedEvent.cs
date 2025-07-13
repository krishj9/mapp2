using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class UserProfileUpdatedEvent : BaseEvent
{
    public User User { get; }

    public UserProfileUpdatedEvent(User user)
    {
        User = user;
    }
}
