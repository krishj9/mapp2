using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.UserManagement.Domain.Events;
using MAPP.Modules.UserManagement.Domain.Enums;

namespace MAPP.Modules.UserManagement.Domain.Entities;

/// <summary>
/// User aggregate root following DDD patterns from Ardalis template
/// </summary>
public class User : BaseAuditableEntity
{
    private readonly List<UserRole> _roles = new();

    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public DateTimeOffset? LastLoginAt { get; private set; }
    public string? Department { get; private set; }
    public string? JobTitle { get; private set; }

    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();
    public string FullName => $"{FirstName} {LastName}";

    // Private constructor for EF Core
    private User() { }

    public User(string email, string firstName, string lastName)
    {
        Email = Guard.Against.NullOrEmpty(email, nameof(email));
        FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        Status = UserStatus.Active;

        AddDomainEvent(new UserCreatedEvent(this));
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber = null, string? department = null, string? jobTitle = null)
    {
        FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        PhoneNumber = phoneNumber;
        Department = department;
        JobTitle = jobTitle;

        AddDomainEvent(new UserProfileUpdatedEvent(this));
    }

    public void UpdateEmail(string email)
    {
        var oldEmail = Email;
        Email = Guard.Against.NullOrEmpty(email, nameof(email));

        AddDomainEvent(new UserEmailUpdatedEvent(this, oldEmail));
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
        {
            throw new InvalidOperationException("User is already active");
        }

        Status = UserStatus.Active;
        AddDomainEvent(new UserActivatedEvent(this));
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
        {
            throw new InvalidOperationException("User is already inactive");
        }

        Status = UserStatus.Inactive;
        AddDomainEvent(new UserDeactivatedEvent(this));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new UserLoggedInEvent(this));
    }

    public void AssignRole(Role role)
    {
        if (_roles.Any(ur => ur.RoleId == role.Id))
        {
            throw new InvalidOperationException($"User already has role: {role.Name}");
        }

        var userRole = new UserRole(Id, role.Id);
        _roles.Add(userRole);

        AddDomainEvent(new UserRoleAssignedEvent(this, role));
    }

    public void RemoveRole(Role role)
    {
        var userRole = _roles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole == null)
        {
            throw new InvalidOperationException($"User does not have role: {role.Name}");
        }

        _roles.Remove(userRole);
        AddDomainEvent(new UserRoleRemovedEvent(this, role));
    }
}
