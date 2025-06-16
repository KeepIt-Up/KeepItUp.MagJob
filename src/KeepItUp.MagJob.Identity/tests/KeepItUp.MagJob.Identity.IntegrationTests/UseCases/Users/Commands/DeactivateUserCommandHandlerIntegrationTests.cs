using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Commands;

/// <summary>
/// Integration tests for DeactivateUserCommandHandler.
/// </summary>
public class DeactivateUserCommandHandlerIntegrationTests : BaseIntegrationTest
{
    public DeactivateUserCommandHandlerIntegrationTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Handle_ActiveUser_ShouldDeactivateUserInDatabase()
    {
        // Arrange
        var activeUser = Core.UserAggregate.User.Create(
            "Active",
            "User",
            "active@example.com",
            "active",
            Guid.NewGuid(),
            true); // Explicitly active

        await DbContext.Users.AddAsync(activeUser);
        await DbContext.SaveChangesAsync();

        var command = new DeactivateUserCommand
        {
            Id = activeUser.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify user was deactivated in database
        var deactivatedUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == activeUser.Id);

        deactivatedUser.Should().NotBeNull();
        deactivatedUser!.IsActive.Should().BeFalse();
        deactivatedUser.Email.Should().Be("active@example.com"); // Other fields unchanged
        deactivatedUser.FirstName.Should().Be("Active");
        deactivatedUser.LastName.Should().Be("User");
    }

    [Fact]
    public async Task Handle_InactiveUser_ShouldRemainInactive()
    {
        // Arrange
        var inactiveUser = Core.UserAggregate.User.Create(
            "Inactive",
            "User",
            "inactive@example.com",
            "inactive",
            Guid.NewGuid(),
            false); // Already inactive

        await DbContext.Users.AddAsync(inactiveUser);
        await DbContext.SaveChangesAsync();

        var command = new DeactivateUserCommand
        {
            Id = inactiveUser.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify user remains inactive (idempotent operation)
        var stillInactiveUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == inactiveUser.Id);

        stillInactiveUser.Should().NotBeNull();
        stillInactiveUser!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var command = new DeactivateUserCommand
        {
            Id = nonExistentUserId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(e => e.Contains("Nie znaleziono użytkownika"));
    }

    [Fact]
    public async Task Handle_UserWithProfile_ShouldDeactivateButKeepProfile()
    {
        // Arrange
        var userWithProfile = Core.UserAggregate.User.Create(
            "Profile",
            "User",
            "profile@example.com",
            "profile",
            Guid.NewGuid());

        // Add profile
        userWithProfile.UpdateProfile(
            "+48123456789",
            "123 Profile Street",
            "https://example.com/profile.jpg");

        await DbContext.Users.AddAsync(userWithProfile);
        await DbContext.SaveChangesAsync();

        var command = new DeactivateUserCommand
        {
            Id = userWithProfile.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify user was deactivated but profile remains
        var deactivatedUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userWithProfile.Id);

        deactivatedUser.Should().NotBeNull();
        deactivatedUser!.IsActive.Should().BeFalse();

        // Profile should remain intact
        deactivatedUser.Profile.Should().NotBeNull();
        deactivatedUser.Profile!.PhoneNumber.Should().Be("+48123456789");
        deactivatedUser.Profile.Address.Should().Be("123 Profile Street");
        deactivatedUser.Profile.ProfileImage.Should().Be("https://example.com/profile.jpg");
    }

    [Fact]
    public async Task Handle_MultipleUsers_ShouldDeactivateOnlyTargetUser()
    {
        // Arrange
        var user1 = Core.UserAggregate.User.Create(
            "User",
            "One",
            "user1@example.com",
            "user1",
            Guid.NewGuid());

        var user2 = Core.UserAggregate.User.Create(
            "User",
            "Two",
            "user2@example.com",
            "user2",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(user1, user2);
        await DbContext.SaveChangesAsync();

        var command = new DeactivateUserCommand
        {
            Id = user1.Id // Only deactivate user1
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify only user1 was deactivated
        var updatedUser1 = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == user1.Id);
        var updatedUser2 = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == user2.Id);

        updatedUser1.Should().NotBeNull();
        updatedUser1!.IsActive.Should().BeFalse();

        updatedUser2.Should().NotBeNull();
        updatedUser2!.IsActive.Should().BeTrue(); // Should remain active
    }
}