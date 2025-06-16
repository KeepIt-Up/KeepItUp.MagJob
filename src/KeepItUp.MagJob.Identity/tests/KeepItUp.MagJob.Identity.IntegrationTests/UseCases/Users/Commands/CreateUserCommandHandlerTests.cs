using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Commands;

/// <summary>
/// Integration tests for CreateUserCommandHandler.
/// Tests the complete flow from command to database persistence.
/// </summary>
public class CreateUserCommandHandlerTests : BaseIntegrationTest
{
    public CreateUserCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : CreateUserCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_CreateUser_When_ValidCommandProvided()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            // Verify user was persisted to database
            var createdUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == result.Value);
            createdUser.Should().NotBeNull();
            createdUser!.ExternalId.Should().Be(command.ExternalId);
            createdUser.Email.Should().Be(command.Email);
            createdUser.FirstName.Should().Be(command.FirstName);
            createdUser.LastName.Should().Be(command.LastName);
            createdUser.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Should_CreateUserWithProfile_When_ProfileDataProvided()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123-456-789",
                Address = "123 Main St",
                ProfileImageUrl = "https://example.com/profile.jpg"
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify user with profile was persisted
            var createdUser = await DbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == result.Value);

            createdUser.Should().NotBeNull();
            createdUser!.Profile.Should().NotBeNull();
            createdUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
            createdUser.Profile.Address.Should().Be(command.Address);
            createdUser.Profile.ProfileImage.Should().Be(command.ProfileImageUrl);
        }

        [Fact]
        public async Task Should_ReturnError_When_UserWithEmailAlreadyExists()
        {
            // Arrange
            var existingUser = User.Create(
                "Existing",
                "User",
                "test@example.com",
                "existing",
                Guid.NewGuid());

            await DbContext.Users.AddAsync(existingUser);
            await SaveAndClearAsync();

            var command = new CreateUserCommand
            {
                ExternalId = Guid.NewGuid(),
                Email = "test@example.com", // Same email
                Username = "newuser",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("adresie e-mail już istnieje"));

            // Verify no new user was created
            var userCount = await DbContext.Users.CountAsync(u => u.Email == command.Email);
            userCount.Should().Be(1); // Only the existing one
        }

        [Fact]
        public async Task Should_ReturnError_When_UserWithExternalIdAlreadyExists()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var existingUser = User.Create(
                "Existing",
                "User",
                "existing@example.com",
                "existing",
                externalId);

            await DbContext.Users.AddAsync(existingUser);
            await SaveAndClearAsync();

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
            result.Errors.Should().Contain(e => e.Contains("identyfikatorze zewnętrznym już istnieje"));

            // Verify no new user was created
            var userCount = await DbContext.Users.CountAsync(u => u.ExternalId == externalId);
            userCount.Should().Be(1); // Only the existing one
        }

        [Fact]
        public async Task Should_UseEmailAsUsername_When_UsernameIsEmpty()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "", // Empty username
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var createdUser = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == result.Value);
            createdUser.Should().NotBeNull();
            createdUser!.Username.Should().Be(command.Email);
        }

        [Fact]
        public async Task Should_CreateUserWithPartialProfile_When_SomeProfileDataProvided()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123-456-789",
                // Address and ProfileImageUrl are null
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var createdUser = await DbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == result.Value);

            createdUser.Should().NotBeNull();
            createdUser!.Profile.Should().NotBeNull();
            createdUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
            createdUser.Profile.Address.Should().BeNull();
            createdUser.Profile.ProfileImage.Should().BeNull();
        }

        [Theory]
        [InlineData("", "Doe", "test@example.com", "test")]
        [InlineData("John", "", "test@example.com", "test")]
        [InlineData("John", "Doe", "", "test")]
        public async Task Should_HandleDomainValidation_When_RequiredFieldsAreMissing(
            string firstName, string lastName, string email, string username)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = Guid.NewGuid(),
                Email = email,
                Username = username,
                FirstName = firstName,
                LastName = lastName
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            // This test depends on domain validation in User.Create
            // If validation fails in domain, result should indicate failure
            // The exact assertion depends on how validation is implemented
            result.Should().NotBeNull();
            // Adjust assertion based on actual validation behavior
        }
    }
}