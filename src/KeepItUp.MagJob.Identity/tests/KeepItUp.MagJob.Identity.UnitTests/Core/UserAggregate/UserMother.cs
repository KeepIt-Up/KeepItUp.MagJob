using KeepItUp.MagJob.Identity.Core.UserAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;

/// <summary>
/// Object Mother pattern implementation for User aggregate.
/// Provides pre-configured User instances for testing.
/// </summary>
public static class UserMother
{
    /// <summary>
    /// Creates a basic valid user with default values.
    /// </summary>
    public static User ValidUser() => User.Create(
        firstName: "John",
        lastName: "Doe",
        email: "john.doe@example.com",
        username: "john.doe",
        externalId: Guid.NewGuid(),
        isActive: true);

    /// <summary>
    /// Creates a user with custom name.
    /// </summary>
    public static User UserWithName(string firstName, string lastName) => User.Create(
        firstName: firstName,
        lastName: lastName,
        email: $"{firstName.ToLower()}.{lastName.ToLower()}@example.com",
        username: $"{firstName.ToLower()}.{lastName.ToLower()}",
        externalId: Guid.NewGuid(),
        isActive: true);

    /// <summary>
    /// Creates a user with custom email.
    /// </summary>
    public static User UserWithEmail(string email) => User.Create(
        firstName: "John",
        lastName: "Doe",
        email: email,
        username: email,
        externalId: Guid.NewGuid(),
        isActive: true);

    /// <summary>
    /// Creates a user with custom external ID.
    /// </summary>
    public static User UserWithExternalId(Guid externalId) => User.Create(
        firstName: "John",
        lastName: "Doe",
        email: "john.doe@example.com",
        username: "john.doe",
        externalId: externalId,
        isActive: true);

    /// <summary>
    /// Creates an inactive user.
    /// </summary>
    public static User InactiveUser() => User.Create(
        firstName: "Jane",
        lastName: "Smith",
        email: "jane.smith@example.com",
        username: "jane.smith",
        externalId: Guid.NewGuid(),
        isActive: false);

    /// <summary>
    /// Creates a user with profile information.
    /// </summary>
    public static User UserWithProfile()
    {
        var user = ValidUser();
        user.UpdateProfile(
            phoneNumber: "123-456-789",
            address: "123 Main Street, City, State 12345",
            profileImage: "https://example.com/profile.jpg");
        return user;
    }

    /// <summary>
    /// Creates a user with permissions.
    /// </summary>
    public static User UserWithPermissions(params string[] permissions)
    {
        var user = ValidUser();
        user.UpdatePermissions(permissions.ToList());
        return user;
    }

    /// <summary>
    /// Creates a user with admin permissions.
    /// </summary>
    public static User AdminUser()
    {
        var user = ValidUser();
        user.UpdatePermissions(new List<string>
        {
            "user.create",
            "user.update",
            "user.delete",
            "organization.create",
            "organization.update",
            "organization.delete"
        });
        return user;
    }

    /// <summary>
    /// Creates a user for testing edge cases.
    /// </summary>
    public static User UserForEdgeCases() => User.Create(
        firstName: "A", // Minimum length
        lastName: "B",  // Minimum length
        email: "a@b.co", // Short but valid email
        username: "ab",
        externalId: Guid.NewGuid(),
        isActive: true);

    /// <summary>
    /// Creates multiple users for batch testing.
    /// </summary>
    public static List<User> MultipleUsers(int count = 3)
    {
        var users = new List<User>();
        for (int i = 0; i < count; i++)
        {
            users.Add(User.Create(
                firstName: $"User{i}",
                lastName: $"Test{i}",
                email: $"user{i}@example.com",
                username: $"user{i}",
                externalId: Guid.NewGuid(),
                isActive: true));
        }
        return users;
    }
}