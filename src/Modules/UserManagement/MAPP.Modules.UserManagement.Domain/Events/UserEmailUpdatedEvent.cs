using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

public class UserEmailUpdatedEvent : BaseEvent
{
    public User User { get; }
    public string OldEmail { get; }

    public UserEmailUpdatedEvent(User user, string oldEmail)
    {
        User = user;
        OldEmail = oldEmail;
    }
}
