namespace MAPP.Modules.UserManagement.Domain.Enums;

/// <summary>
/// User status enumeration following clean architecture patterns
/// </summary>
public enum UserStatus
{
    Active = 0,
    Inactive = 1,
    Suspended = 2,
    PendingActivation = 3
}
