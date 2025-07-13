using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class UserLoggedInEvent : BaseEvent
{
    public User User { get; }

    public UserLoggedInEvent(User user)
    {
        User = user;
    }
}
