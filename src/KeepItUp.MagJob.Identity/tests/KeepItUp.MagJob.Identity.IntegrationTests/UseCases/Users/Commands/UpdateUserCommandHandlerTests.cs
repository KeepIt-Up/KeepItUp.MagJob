using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Commands;

/// <summary>
/// Integration tests for UpdateUserCommandHandler.
/// Tests the complete flow from command to database persistence.
/// </summary>
public class UpdateUserCommandHandlerTests : BaseIntegrationTest
{
    public UpdateUserCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : UpdateUserCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_UpdateUser_When_ValidCommandProvided()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "123-456-789",
                Address = "123 Main St",
                ProfileImageUrl = "https://example.com/profile.jpg"
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify user was updated in database
            var updatedUser = await DbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            updatedUser.Should().NotBeNull();
            updatedUser!.FirstName.Should().Be(command.FirstName);
            updatedUser.LastName.Should().Be(command.LastName);
            updatedUser.Profile.Should().NotBeNull();
            updatedUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
            updatedUser.Profile.Address.Should().Be(command.Address);
            updatedUser.Profile.ProfileImage.Should().Be(command.ProfileImageUrl);
        }

        [Fact]
        public async Task Should_UpdateProfileFull_When_NullValuesProvided()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            // Set initial profile data
            user.UpdateProfile("999-888-777", "Old Address", "old-image.jpg");

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = null, // Should clear phone number
                Address = null, // Should clear address
                ProfileImageUrl = null // Should clear profile image
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var updatedUser = await DbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            updatedUser.Should().NotBeNull();
            updatedUser!.FirstName.Should().Be(command.FirstName);
            updatedUser.LastName.Should().Be(command.LastName);
            updatedUser.Profile.Should().NotBeNull();
            updatedUser.Profile!.PhoneNumber.Should().BeNull();
            updatedUser.Profile.Address.Should().BeNull();
            updatedUser.Profile.ProfileImage.Should().BeNull();
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var command = new UpdateUserCommand
            {
                Id = nonExistentUserId,
                FirstName = "Jane",
                LastName = "Smith"
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o ID {nonExistentUserId}"));
        }

        [Fact]
        public async Task Should_UpdateUserWithoutProfile_When_NoProfileDataProvided()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "Jane",
                LastName = "Smith"
                // No profile data provided
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var updatedUser = await DbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            updatedUser.Should().NotBeNull();
            updatedUser!.FirstName.Should().Be(command.FirstName);
            updatedUser.LastName.Should().Be(command.LastName);
            updatedUser.Profile.Should().NotBeNull(); // Profile should be created with null values
        }

        [Fact]
        public async Task Should_UpdatePartialProfile_When_SomeProfileDataProvided()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "123-456-789"
                // Address and ProfileImageUrl are null
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var updatedUser = await DbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            updatedUser.Should().NotBeNull();
            updatedUser!.FirstName.Should().Be(command.FirstName);
            updatedUser.LastName.Should().Be(command.LastName);
            updatedUser.Profile.Should().NotBeNull();
            updatedUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
            updatedUser.Profile.Address.Should().BeNull();
            updatedUser.Profile.ProfileImage.Should().BeNull();
        }

        [Fact]
        public async Task Should_PreserveOtherUserData_When_UpdateSuccessful()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var originalEmail = user.Email;
            var originalUsername = user.Username;
            var originalExternalId = user.ExternalId;
            var originalIsActive = user.IsActive;

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "Jane",
                LastName = "Smith"
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var updatedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser!.Email.Should().Be(originalEmail);
            updatedUser.Username.Should().Be(originalUsername);
            updatedUser.ExternalId.Should().Be(originalExternalId);
            updatedUser.IsActive.Should().Be(originalIsActive);
        }
    }
}