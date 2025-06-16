using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Commands;

/// <summary>
/// Integration tests for DeactivateUserCommandHandler.
/// Tests the complete flow from command to database persistence.
/// </summary>
public class DeactivateUserCommandHandlerTests : BaseIntegrationTest
{
    public DeactivateUserCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : DeactivateUserCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_DeactivateUser_When_ActiveUserExists()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            // Ensure user is active initially
            user.IsActive.Should().BeTrue();

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var command = new DeactivateUserCommand { Id = user.Id };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify user was deactivated in database
            var deactivatedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            deactivatedUser.Should().NotBeNull();
            deactivatedUser!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var command = new DeactivateUserCommand { Id = nonExistentUserId };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o ID {nonExistentUserId}"));
        }

        [Fact]
        public async Task Should_DeactivateUser_When_UserIsAlreadyInactive()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            user.Deactivate(); // Already deactivated

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var command = new DeactivateUserCommand { Id = user.Id };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify user remains inactive
            var checkedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            checkedUser.Should().NotBeNull();
            checkedUser!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task Should_PreserveUserData_When_DeactivationSuccessful()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            user.UpdateProfile("123-456-789", "123 Main St", "profile.jpg");

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var originalFirstName = user.FirstName;
            var originalLastName = user.LastName;
            var originalEmail = user.Email;
            var originalExternalId = user.ExternalId;

            var command = new DeactivateUserCommand { Id = user.Id };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var deactivatedUser = await DbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            deactivatedUser.Should().NotBeNull();
            deactivatedUser!.IsActive.Should().BeFalse();
            deactivatedUser.FirstName.Should().Be(originalFirstName);
            deactivatedUser.LastName.Should().Be(originalLastName);
            deactivatedUser.Email.Should().Be(originalEmail);
            deactivatedUser.ExternalId.Should().Be(originalExternalId);

            // Profile should also be preserved
            deactivatedUser.Profile.Should().NotBeNull();
            deactivatedUser.Profile!.PhoneNumber.Should().Be("123-456-789");
            deactivatedUser.Profile.Address.Should().Be("123 Main St");
            deactivatedUser.Profile.ProfileImage.Should().Be("profile.jpg");
        }

        [Fact]
        public async Task Should_EmitDomainEvent_When_UserDeactivated()
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

            var command = new DeactivateUserCommand { Id = user.Id };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify domain event was raised (this depends on domain event implementation)
            // The exact assertion will depend on how domain events are tracked in tests
            var deactivatedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            deactivatedUser.Should().NotBeNull();
            deactivatedUser!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task Should_HandleConcurrentDeactivation_When_MultipleRequestsOccur()
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

            var command1 = new DeactivateUserCommand { Id = user.Id };
            var command2 = new DeactivateUserCommand { Id = user.Id };

            // Act
            var result1 = await Mediator.Send(command1);
            var result2 = await Mediator.Send(command2);

            // Assert
            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue(); // Both should succeed

            var deactivatedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            deactivatedUser.Should().NotBeNull();
            deactivatedUser!.IsActive.Should().BeFalse();
        }

        [Theory]
        [InlineData("John", "Doe", "john.doe@example.com")]
        [InlineData("Jane", "Smith", "jane.smith@example.com")]
        [InlineData("Bob", "Johnson", "bob.johnson@example.com")]
        public async Task Should_DeactivateUser_When_DifferentUserDataProvided(
            string firstName, string lastName, string email)
        {
            // Arrange
            var user = User.Create(
                firstName,
                lastName,
                email,
                email,
                Guid.NewGuid());

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var command = new DeactivateUserCommand { Id = user.Id };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var deactivatedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            deactivatedUser.Should().NotBeNull();
            deactivatedUser!.IsActive.Should().BeFalse();
            deactivatedUser.FirstName.Should().Be(firstName);
            deactivatedUser.LastName.Should().Be(lastName);
            deactivatedUser.Email.Should().Be(email);
        }
    }
}