using KeepItUp.MagJob.Identity.Core.UserAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;

/// <summary>
/// Builder pattern implementation for User aggregate.
/// Provides fluent API for creating User instances in tests.
/// </summary>
public class UserBuilder
{
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string _email = "john.doe@example.com";
    private string _username = "john.doe";
    private Guid _externalId = Guid.NewGuid();
    private bool _isActive = true;
    private string? _phoneNumber;
    private string? _address;
    private string? _profileImage;
    private List<string> _permissions = new();

    /// <summary>
    /// Sets user's first and last name.
    /// </summary>
    public UserBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        // Update email and username to match the name
        _email = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com";
        _username = $"{firstName.ToLower()}.{lastName.ToLower()}";
        return this;
    }

    /// <summary>
    /// Sets user's first name.
    /// </summary>
    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    /// <summary>
    /// Sets user's last name.
    /// </summary>
    public UserBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    /// <summary>
    /// Sets user's email address.
    /// </summary>
    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    /// <summary>
    /// Sets user's username.
    /// </summary>
    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    /// <summary>
    /// Sets user's external ID.
    /// </summary>
    public UserBuilder WithExternalId(Guid externalId)
    {
        _externalId = externalId;
        return this;
    }

    /// <summary>
    /// Sets user as active.
    /// </summary>
    public UserBuilder AsActive()
    {
        _isActive = true;
        return this;
    }

    /// <summary>
    /// Sets user as inactive.
    /// </summary>
    public UserBuilder AsInactive()
    {
        _isActive = false;
        return this;
    }

    /// <summary>
    /// Sets user's phone number.
    /// </summary>
    public UserBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    /// <summary>
    /// Sets user's address.
    /// </summary>
    public UserBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    /// <summary>
    /// Sets user's profile image.
    /// </summary>
    public UserBuilder WithProfileImage(string profileImage)
    {
        _profileImage = profileImage;
        return this;
    }

    /// <summary>
    /// Sets user's complete profile.
    /// </summary>
    public UserBuilder WithProfile(string? phoneNumber = null, string? address = null, string? profileImage = null)
    {
        _phoneNumber = phoneNumber;
        _address = address;
        _profileImage = profileImage;
        return this;
    }

    /// <summary>
    /// Adds a permission to the user.
    /// </summary>
    public UserBuilder WithPermission(string permission)
    {
        if (!_permissions.Contains(permission))
        {
            _permissions.Add(permission);
        }
        return this;
    }

    /// <summary>
    /// Adds multiple permissions to the user.
    /// </summary>
    public UserBuilder WithPermissions(params string[] permissions)
    {
        foreach (var permission in permissions)
        {
            WithPermission(permission);
        }
        return this;
    }

    /// <summary>
    /// Adds permissions from a list.
    /// </summary>
    public UserBuilder WithPermissions(List<string> permissions)
    {
        foreach (var permission in permissions)
        {
            WithPermission(permission);
        }
        return this;
    }

    /// <summary>
    /// Sets up user with admin permissions.
    /// </summary>
    public UserBuilder AsAdmin()
    {
        _permissions = new List<string>
        {
            "user.create",
            "user.update",
            "user.delete",
            "organization.create",
            "organization.update",
            "organization.delete"
        };
        return this;
    }

    /// <summary>
    /// Clears all permissions.
    /// </summary>
    public UserBuilder WithoutPermissions()
    {
        _permissions.Clear();
        return this;
    }

    /// <summary>
    /// Builds the User instance.
    /// </summary>
    public User Build()
    {
        var user = User.Create(
            firstName: _firstName,
            lastName: _lastName,
            email: _email,
            username: _username,
            externalId: _externalId,
            isActive: _isActive);

        // Add profile if any profile data is set
        if (!string.IsNullOrEmpty(_phoneNumber) ||
            !string.IsNullOrEmpty(_address) ||
            !string.IsNullOrEmpty(_profileImage))
        {
            user.UpdateProfile(_phoneNumber, _address, _profileImage);
        }

        // Add permissions if any are set
        if (_permissions.Any())
        {
            user.UpdatePermissions(_permissions);
        }

        return user;
    }

    /// <summary>
    /// Creates a new UserBuilder with default values.
    /// </summary>
    public static UserBuilder New() => new UserBuilder();

    /// <summary>
    /// Creates a new UserBuilder based on UserMother.ValidUser().
    /// </summary>
    public static UserBuilder Valid() => new UserBuilder();

    /// <summary>
    /// Creates multiple users using the current builder configuration.
    /// Each user will have a unique external ID and modified email/username.
    /// </summary>
    public List<User> BuildMany(int count)
    {
        var users = new List<User>();
        for (int i = 0; i < count; i++)
        {
            var builder = new UserBuilder
            {
                _firstName = _firstName,
                _lastName = _lastName,
                _email = $"{_firstName.ToLower()}.{_lastName.ToLower()}{i}@example.com",
                _username = $"{_firstName.ToLower()}.{_lastName.ToLower()}{i}",
                _externalId = Guid.NewGuid(),
                _isActive = _isActive,
                _phoneNumber = _phoneNumber,
                _address = _address,
                _profileImage = _profileImage,
                _permissions = new List<string>(_permissions)
            };
            users.Add(builder.Build());
        }
        return users;
    }
}