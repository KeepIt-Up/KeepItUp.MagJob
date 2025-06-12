using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// User ID in the external system (Keycloak).
    /// </summary>
    public Guid ExternalId { get; private set; }

    /// <summary>
    /// User email address.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// User first name.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// User last name.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// User profile.
    /// </summary>
    public UserProfile? Profile { get; private set; }

    /// <summary>
    /// Whether the user is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// User username.
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// List of user permissions.
    /// </summary>
    private readonly List<string> _permissions = new();

    /// <summary>
    /// List of organizations, to which the user belongs.
    /// </summary>
    private readonly List<Member> _memberships = new();

    /// <summary>
    /// List of user permissions (read-only).
    /// </summary>
    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    /// <summary>
    /// List of organizations, to which the user belongs (read-only).
    /// </summary>
    public IReadOnlyCollection<Member> Memberships => _memberships.AsReadOnly();

    /// <summary>
    /// User last login date.
    /// </summary>
    public DateTime LastLoginDate { get; private set; } = DateTime.MinValue;

    /// <summary>
    /// Private constructor for EF Core and factory creation.
    /// </summary>
    private User() { }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="email">Email address</param>
    /// <param name="username">Username</param>
    /// <param name="externalId">External ID</param>
    /// <param name="isActive">Whether the user is active</param>
    /// <returns>New user</returns>
    public static User Create(string firstName, string lastName, string email, string username, Guid externalId, bool isActive = true)
    {
        Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.NullOrEmpty(username, nameof(username));
        Guard.Against.Default(externalId, nameof(externalId));

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username,
            ExternalId = externalId,
            IsActive = isActive,
        };

        user.RegisterDomainEventAndUpdate(new UserCreatedEvent(user.Id, user.ExternalId, user.Email));

        return user;
    }

    /// <summary>
    /// Updates the user's data.
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    public void Update(string firstName, string lastName)
    {
        Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        Guard.Against.NullOrEmpty(lastName, nameof(lastName));

        // Check if the values actually changed
        if (firstName == FirstName && lastName == LastName)
        {
            return; // No changes, do not update and do not emit events
        }

        FirstName = firstName;
        LastName = lastName;

        RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Updates the user's profile.
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="address">Address</param>
    /// <param name="profileImage">Profile picture URL</param>
    public void UpdateProfile(string? phoneNumber, string? address, string? profileImage)
    {
        // Check if the values actually changed
        if (Profile is not null &&
            string.Equals(phoneNumber, Profile.PhoneNumber) &&
            string.Equals(address, Profile.Address) &&
            string.Equals(profileImage, Profile.ProfileImage))
        {
            return; // No changes, do not update and do not emit events
        }

        Profile = new UserProfile(phoneNumber, address, profileImage);

        RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Updates the user's profile properties.
    /// </summary>
    /// <param name="phoneNumber">New phone number or null to keep the current one.</param>
    /// <param name="address">New address or null to keep the current one.</param>
    /// <param name="profileImage">New profile picture URL or null to keep the current one.</param>
    public void UpdateProfileProperties(string? phoneNumber = null, string? address = null, string? profileImage = null)
    {
        // If the profile does not exist, create a new one only if some values are provided
        if (Profile is null)
        {
            // Do not create a profile if all values are empty
            if (string.IsNullOrEmpty(phoneNumber) &&
                string.IsNullOrEmpty(address) &&
                string.IsNullOrEmpty(profileImage))
            {
                return; // No profile and no data to set, so there are no changes
            }

            Profile = new UserProfile(phoneNumber, address, profileImage);
            RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
            return;
        }

        // Update the profile and check if there are any changes
        var updatedProfile = Profile.WithUpdates(phoneNumber, address, profileImage);

        // If the profile has changed (based on values)
        if (!updatedProfile.Equals(Profile))
        {
            Profile = updatedProfile;
            RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
        }
    }

    /// <summary>
    /// Updates the user's permissions.
    /// </summary>
    /// <param name="permissions">List of permissions</param>
    public void UpdatePermissions(List<string> permissions)
    {
        _permissions.Clear();
        if (permissions != null)
        {
            _permissions.AddRange(permissions);
        }

        RegisterDomainEventAndUpdate(new UserPermissionsUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Adds a permission to the user.
    /// </summary>
    /// <param name="permission">Permission to add</param>
    /// <returns>True, if the permission was added; false, if it already exists</returns>
    public bool AddPermission(string permission)
    {
        Guard.Against.NullOrEmpty(permission, nameof(permission));

        if (_permissions.Contains(permission))
        {
            return false;
        }

        _permissions.Add(permission);

        RegisterDomainEventAndUpdate(new UserPermissionsUpdatedEvent(Id, ExternalId, Email));
        return true;
    }

    /// <summary>
    /// Removes a permission from the user.
    /// </summary>
    /// <param name="permission">Permission to remove</param>
    /// <returns>True, if the permission was removed; false, if it did not exist</returns>
    public bool RemovePermission(string permission)
    {
        Guard.Against.NullOrEmpty(permission, nameof(permission));

        if (!_permissions.Contains(permission))
        {
            return false;
        }

        _permissions.Remove(permission);

        RegisterDomainEventAndUpdate(new UserPermissionsUpdatedEvent(Id, ExternalId, Email));
        return true;
    }

    /// <summary>
    /// Checks if the user has a specific permission.
    /// </summary>
    /// <param name="permission">Permission to check</param>
    /// <returns>True, if the user has the permission; otherwise false</returns>
    public bool HasPermission(string permission)
    {
        Guard.Against.NullOrEmpty(permission, nameof(permission));
        return _permissions.Contains(permission);
    }

    /// <summary>
    /// Updates the user's last login date.
    /// </summary>
    /// <param name="lastLoginDate">Last login date</param>
    public void UpdateLastLoginDate(DateTime lastLoginDate)
    {
        LastLoginDate = lastLoginDate;
        RegisterDomainEventAndUpdate(new UserLastLoginUpdatedEvent(Id, ExternalId, Email, lastLoginDate));
    }

    /// <summary>
    /// Decciivatss  he user.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;


        RegisterDomainEventAndUpdate(new UserDeactivatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Activates the user.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;


        RegisterDomainEventAndUpdate(new UserActivatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Updates the user's email and status.
    /// </summary>
    /// <param name="email">New email address</param>
    /// <param name="isActive">New status</param>
    public void UpdateEmailAndStatus(string email, bool isActive)
    {
        Guard.Against.NullOrEmpty(email, nameof(email));

        Email = email;

        // Update the status
        if (IsActive != isActive)
        {
            IsActive = isActive;

            if (isActive)
            {
                RegisterDomainEventAndUpdate(new UserActivatedEvent(Id, ExternalId, Email));
            }
            else
            {
                RegisterDomainEventAndUpdate(new UserDeactivatedEvent(Id, ExternalId, Email));
            }
        }
        else
        {
            RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
        }
    }

    /// <summary>
    /// Updates all user data.
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="email">Email address</param>
    /// <param name="username">Username</param>
    /// <param name="isActive">Whether the user is active</param>
    public void UpdateAllDetails(string firstName, string lastName, string email, string username, bool isActive)
    {
        Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.NullOrEmpty(username, nameof(username));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Username = username;

        // Update the status
        if (IsActive != isActive)
        {
            IsActive = isActive;

            if (isActive)
            {
                RegisterDomainEventAndUpdate(new UserActivatedEvent(Id, ExternalId, Email));
            }
            else
            {
                RegisterDomainEventAndUpdate(new UserDeactivatedEvent(Id, ExternalId, Email));
            }
        }
        else
        {
            RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
        }
    }
}
