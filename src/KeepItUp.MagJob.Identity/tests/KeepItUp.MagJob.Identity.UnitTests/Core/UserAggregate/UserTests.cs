using FluentAssertions;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;

/// <summary>
/// Simplified unit tests for User aggregate.
/// Tests core business logic without complex domain event management.
/// </summary>
public class UserTests
{
    /// <summary>
    /// Tests for User creation.
    /// </summary>
    public class Create
    {
        [Fact]
        public void Should_CreateUser_When_ValidDataProvided()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";
            var username = "john.doe";
            var externalId = Guid.NewGuid();

            // Act
            var user = User.Create(firstName, lastName, email, username, externalId);

            // Assert
            user.Should().NotBeNull();
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().Be(email);
            user.Username.Should().Be(username);
            user.ExternalId.Should().Be(externalId);
            user.IsActive.Should().BeTrue();
            user.Profile.Should().BeNull();
            user.Permissions.Should().BeEmpty();
        }

        [Fact]
        public void Should_CreateInactiveUser_When_IsActiveFalse()
        {
            // Arrange
            var user = UserMother.ValidUser();

            // Act & Assert
            user.IsActive.Should().BeTrue(); // Default from UserMother

            // Create inactive user
            var inactiveUser = User.Create("Jane", "Smith", "jane@example.com", "jane", Guid.NewGuid(), false);
            inactiveUser.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_EmitUserCreatedEvent_When_UserCreated()
        {
            // Arrange & Act
            var user = UserMother.ValidUser();

            // Assert
            user.DomainEvents.Should().NotBeEmpty();
            user.DomainEvents.Should().Contain(e => e is UserCreatedEvent);
        }

        [Theory]
        [InlineData("", "Doe", "john@example.com", "john")]
        [InlineData("John", "", "john@example.com", "john")]
        [InlineData("John", "Doe", "", "john")]
        [InlineData("John", "Doe", "john@example.com", "")]
        public void Should_ThrowArgumentException_When_RequiredFieldsEmpty(string firstName, string lastName, string email, string username)
        {
            // Arrange
            var externalId = Guid.NewGuid();

            // Act & Assert
            var action = () => User.Create(firstName, lastName, email, username, externalId);
            action.Should().Throw<ArgumentException>();
        }
    }

    /// <summary>
    /// Tests for User updates.
    /// </summary>
    public class Update
    {
        [Fact]
        public void Should_UpdateUser_When_ValidDataProvided()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var newFirstName = "Jane";
            var newLastName = "Smith";

            // Act
            user.Update(newFirstName, newLastName);

            // Assert
            user.FirstName.Should().Be(newFirstName);
            user.LastName.Should().Be(newLastName);
        }

        [Theory]
        [InlineData("", "Smith")]
        [InlineData("Jane", "")]
        public void Should_ThrowArgumentException_When_UpdateDataInvalid(string firstName, string lastName)
        {
            // Arrange
            var user = UserMother.ValidUser();

            // Act & Assert
            var action = () => user.Update(firstName, lastName);
            action.Should().Throw<ArgumentException>();
        }
    }

    /// <summary>
    /// Tests for User profile management.
    /// </summary>
    public class ProfileManagement
    {
        [Fact]
        public void Should_UpdateProfile_When_ValidDataProvided()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var phoneNumber = "123-456-789";
            var address = "123 Main St";
            var profileImage = "profile.jpg";

            // Act
            user.UpdateProfile(phoneNumber, address, profileImage);

            // Assert
            user.Profile.Should().NotBeNull();
            user.Profile!.PhoneNumber.Should().Be(phoneNumber);
            user.Profile.Address.Should().Be(address);
            user.Profile.ProfileImage.Should().Be(profileImage);
        }

        [Fact]
        public void Should_CreateProfile_When_NoneExists()
        {
            // Arrange
            var user = UserMother.ValidUser();
            user.Profile.Should().BeNull(); // Ensure no profile initially

            // Act
            user.UpdateProfile("123-456-789", null, null);

            // Assert
            user.Profile.Should().NotBeNull();
            user.Profile!.PhoneNumber.Should().Be("123-456-789");
        }
    }

    /// <summary>
    /// Tests for User permissions.
    /// </summary>
    public class PermissionsManagement
    {
        [Fact]
        public void Should_AddPermission_When_PermissionDoesNotExist()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var permission = "user.create";

            // Act
            var result = user.AddPermission(permission);

            // Assert
            result.Should().BeTrue();
            user.HasPermission(permission).Should().BeTrue();
            user.Permissions.Should().Contain(permission);
        }

        [Fact]
        public void Should_NotAddDuplicatePermission()
        {
            // Arrange
            var user = UserMother.UserWithPermissions("user.create");

            // Act
            var result = user.AddPermission("user.create");

            // Assert
            result.Should().BeFalse();
            user.Permissions.Should().ContainSingle();
        }

        [Fact]
        public void Should_RemovePermission_When_PermissionExists()
        {
            // Arrange
            var user = UserMother.UserWithPermissions("user.create", "user.update");

            // Act
            var result = user.RemovePermission("user.create");

            // Assert
            result.Should().BeTrue();
            user.HasPermission("user.create").Should().BeFalse();
            user.Permissions.Should().Contain("user.update");
        }

        [Fact]
        public void Should_UpdateAllPermissions()
        {
            // Arrange
            var user = UserMother.UserWithPermissions("user.create");
            var newPermissions = new List<string> { "user.update", "user.delete" };

            // Act
            user.UpdatePermissions(newPermissions);

            // Assert
            user.Permissions.Should().BeEquivalentTo(newPermissions);
            user.HasPermission("user.create").Should().BeFalse();
        }
    }

    /// <summary>
    /// Tests for User status management.
    /// </summary>
    public class StatusManagement
    {
        [Fact]
        public void Should_ActivateUser_When_UserIsInactive()
        {
            // Arrange
            var user = UserMother.InactiveUser();

            // Act
            user.Activate();

            // Assert
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_DeactivateUser_When_UserIsActive()
        {
            // Arrange
            var user = UserMother.ValidUser();

            // Act
            user.Deactivate();

            // Assert
            user.IsActive.Should().BeFalse();
        }
    }

    /// <summary>
    /// Tests for UserMother factory methods.
    /// </summary>
    public class UserMotherTests
    {
        [Fact]
        public void Should_CreateValidUser()
        {
            // Act
            var user = UserMother.ValidUser();

            // Assert
            user.Should().NotBeNull();
            user.FirstName.Should().Be("John");
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_CreateUserWithProfile()
        {
            // Act
            var user = UserMother.UserWithProfile();

            // Assert
            user.Profile.Should().NotBeNull();
            user.Profile!.PhoneNumber.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Should_CreateAdminUser()
        {
            // Act
            var user = UserMother.AdminUser();

            // Assert
            user.Permissions.Should().NotBeEmpty();
            user.HasPermission("user.create").Should().BeTrue();
        }
    }

    /// <summary>
    /// Tests for UserBuilder fluent API.
    /// </summary>
    public class UserBuilderTests
    {
        [Fact]
        public void Should_BuildUserWithFluentAPI()
        {
            // Act
            var user = UserBuilder.New()
                .WithName("Alice", "Johnson")
                .WithEmail("alice@example.com")
                .AsActive()
                .WithPermission("user.read")
                .Build();

            // Assert
            user.FirstName.Should().Be("Alice");
            user.LastName.Should().Be("Johnson");
            user.Email.Should().Be("alice@example.com");
            user.IsActive.Should().BeTrue();
            user.HasPermission("user.read").Should().BeTrue();
        }

        [Fact]
        public void Should_BuildMultipleUsers()
        {
            // Act
            var users = UserBuilder.New()
                .WithName("Test", "User")
                .BuildMany(3);

            // Assert
            users.Should().HaveCount(3);
            users.Should().OnlyContain(u => u.FirstName == "Test");
            users.Should().OnlyContain(u => u.LastName == "User");
            users.Select(u => u.ExternalId).Should().OnlyHaveUniqueItems();
        }
    }
}