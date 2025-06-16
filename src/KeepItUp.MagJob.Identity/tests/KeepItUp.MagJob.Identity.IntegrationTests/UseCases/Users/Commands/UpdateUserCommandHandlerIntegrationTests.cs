using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Commands;

/// <summary>
/// Integration tests for UpdateUserCommandHandler.
/// </summary>
public class UpdateUserCommandHandlerIntegrationTests : BaseIntegrationTest
{
    public UpdateUserCommandHandlerIntegrationTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateUserInDatabase()
    {
        // Arrange
        var existingUser = Core.UserAggregate.User.Create(
            "Original",
            "User",
            "original@example.com",
            "original",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(existingUser);
        await DbContext.SaveChangesAsync();

        var command = new UpdateUserCommand
        {
            Id = existingUser.Id,
            FirstName = "Updated",
            LastName = "Name",
            PhoneNumber = "+48987654321",
            Address = "456 Updated Street",
            ProfileImageUrl = "https://example.com/updated-avatar.jpg"
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify user was updated in database
        var updatedUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == existingUser.Id);

        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be(command.FirstName);
        updatedUser.LastName.Should().Be(command.LastName);
        updatedUser.Email.Should().Be("original@example.com"); // Should not change
        updatedUser.Username.Should().Be("original"); // Should not change

        // Verify profile was updated
        updatedUser.Profile.Should().NotBeNull();
        updatedUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
        updatedUser.Profile.Address.Should().Be(command.Address);
        updatedUser.Profile.ProfileImage.Should().Be(command.ProfileImageUrl);
    }

    [Fact]
    public async Task Handle_UserWithExistingProfile_ShouldUpdateProfile()
    {
        // Arrange
        var existingUser = Core.UserAggregate.User.Create(
            "Test",
            "User",
            "test@example.com",
            "test",
            Guid.NewGuid());

        // Add initial profile
        existingUser.UpdateProfile(
            "+48111111111",
            "Old Address",
            "https://example.com/old-avatar.jpg");

        await DbContext.Users.AddAsync(existingUser);
        await DbContext.SaveChangesAsync();

        var command = new UpdateUserCommand
        {
            Id = existingUser.Id,
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "+48222222222",
            Address = "New Address",
            ProfileImageUrl = "https://example.com/new-avatar.jpg"
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify profile was updated
        var updatedUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == existingUser.Id);

        updatedUser.Should().NotBeNull();
        updatedUser!.Profile.Should().NotBeNull();
        updatedUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
        updatedUser.Profile.Address.Should().Be(command.Address);
        updatedUser.Profile.ProfileImage.Should().Be(command.ProfileImageUrl);
    }

    [Fact]
    public async Task Handle_NonExistentUser_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Id = nonExistentUserId,
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(e => e.Contains("Nie znaleziono użytkownika"));
    }

    [Fact]
    public async Task Handle_MinimalUpdate_ShouldUpdateOnlyNameFields()
    {
        // Arrange
        var existingUser = Core.UserAggregate.User.Create(
            "Original",
            "User",
            "original@example.com",
            "original",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(existingUser);
        await DbContext.SaveChangesAsync();

        var command = new UpdateUserCommand
        {
            Id = existingUser.Id,
            FirstName = "Updated",
            LastName = "Name"
            // No profile fields
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify user was updated
        var updatedUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == existingUser.Id);

        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be(command.FirstName);
        updatedUser.LastName.Should().Be(command.LastName);

        // Profile should have empty values since no profile data was provided
        updatedUser.Profile.Should().NotBeNull();
        updatedUser.Profile!.PhoneNumber.Should().BeNullOrEmpty();
        updatedUser.Profile.Address.Should().BeNullOrEmpty();
        updatedUser.Profile.ProfileImage.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_ClearProfileFields_ShouldSetFieldsToNull()
    {
        // Arrange
        var existingUser = Core.UserAggregate.User.Create(
            "Test",
            "User",
            "test@example.com",
            "test",
            Guid.NewGuid());

        // Add initial profile
        existingUser.UpdateProfile(
            "+48111111111",
            "Old Address",
            "https://example.com/old-avatar.jpg");

        await DbContext.Users.AddAsync(existingUser);
        await DbContext.SaveChangesAsync();

        var command = new UpdateUserCommand
        {
            Id = existingUser.Id,
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = null, // Clear phone
            Address = null,     // Clear address
            ProfileImageUrl = null // Clear image
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify profile fields were cleared
        var updatedUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == existingUser.Id);

        updatedUser.Should().NotBeNull();
        updatedUser!.Profile.Should().NotBeNull();
        updatedUser.Profile!.PhoneNumber.Should().BeNullOrEmpty();
        updatedUser.Profile.Address.Should().BeNullOrEmpty();
        updatedUser.Profile.ProfileImage.Should().BeNullOrEmpty();
    }
}