using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Commands;

/// <summary>
/// Integration tests for CreateUserCommandHandler.
/// </summary>
public class CreateUserCommandHandlerIntegrationTests : BaseIntegrationTest
{
    public CreateUserCommandHandlerIntegrationTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUserInDatabase()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            ExternalId = Guid.NewGuid(),
            Email = "test.user@example.com",
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+48123456789",
            Address = "123 Test Street",
            ProfileImageUrl = "https://example.com/avatar.jpg"
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        // Verify user was created in database
        var createdUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == result.Value);

        createdUser.Should().NotBeNull();
        createdUser!.ExternalId.Should().Be(command.ExternalId);
        createdUser.Email.Should().Be(command.Email);
        createdUser.Username.Should().Be(command.Username);
        createdUser.FirstName.Should().Be(command.FirstName);
        createdUser.LastName.Should().Be(command.LastName);
        createdUser.IsActive.Should().BeTrue();
        createdUser.IsDeleted.Should().BeFalse();
        createdUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        // Verify profile was created
        createdUser.Profile.Should().NotBeNull();
        createdUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
        createdUser.Profile.Address.Should().Be(command.Address);
        createdUser.Profile.ProfileImage.Should().Be(command.ProfileImageUrl);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldReturnFailure()
    {
        // Arrange
        var existingUser = Core.UserAggregate.User.Create(
            "Existing",
            "User",
            "existing@example.com",
            "existing",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(existingUser);
        await DbContext.SaveChangesAsync();

        var command = new CreateUserCommand
        {
            ExternalId = Guid.NewGuid(),
            Email = "existing@example.com", // Same email
            Username = "newuser",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(e => e.Contains("adresie e-mail już istnieje"));

        // Verify no new user was created
        var userCount = await DbContext.Users.CountAsync();
        userCount.Should().Be(1); // Only the existing user
    }

    [Fact]
    public async Task Handle_DuplicateExternalId_ShouldReturnFailure()
    {
        // Arrange
        var externalId = Guid.NewGuid();
        var existingUser = Core.UserAggregate.User.Create(
            "Existing",
            "User",
            "existing@example.com",
            "existing",
            externalId);

        await DbContext.Users.AddAsync(existingUser);
        await DbContext.SaveChangesAsync();

        var command = new CreateUserCommand
        {
            ExternalId = externalId, // Same external ID
            Email = "new@example.com",
            Username = "newuser",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain(e => e.Contains("identyfikatorze zewnętrznym już istnieje"));

        // Verify no new user was created
        var userCount = await DbContext.Users.CountAsync();
        userCount.Should().Be(1); // Only the existing user
    }

    [Fact]
    public async Task Handle_MinimalData_ShouldCreateUserWithoutOptionalFields()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            ExternalId = Guid.NewGuid(),
            Email = "minimal@example.com",
            Username = "minimal",
            FirstName = "Min",
            LastName = "User"
            // No optional fields
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        // Verify user was created in database
        var createdUser = await DbContext.Users
            .FirstOrDefaultAsync(u => u.Id == result.Value);

        createdUser.Should().NotBeNull();
        createdUser!.ExternalId.Should().Be(command.ExternalId);
        createdUser.Email.Should().Be(command.Email);
        createdUser.Username.Should().Be(command.Username);
        createdUser.FirstName.Should().Be(command.FirstName);
        createdUser.LastName.Should().Be(command.LastName);

        // Profile should have empty values when no profile data is provided
        createdUser.Profile.Should().NotBeNull();
        createdUser.Profile!.PhoneNumber.Should().BeNullOrEmpty();
        createdUser.Profile.Address.Should().BeNullOrEmpty();
        createdUser.Profile.ProfileImage.Should().BeNullOrEmpty();
    }
}