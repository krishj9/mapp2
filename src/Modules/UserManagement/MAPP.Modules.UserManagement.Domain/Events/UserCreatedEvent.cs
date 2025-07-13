using MAPP.BuildingBlocks.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Entities;

namespace MAPP.Modules.UserManagement.Domain.Events;

/// <summary>
/// Domain event raised when a user is created
/// Following Ardalis Clean Architecture patterns
/// </summary>
public class UserCreatedEvent : BaseEvent
{
    public User User { get; }

    public UserCreatedEvent(User user)
    {
        User = user;
    }
}
