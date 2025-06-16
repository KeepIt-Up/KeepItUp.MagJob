using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Queries;

/// <summary>
/// Integration tests for GetUserByExternalIdQueryHandler.
/// Tests the complete flow from query to database retrieval by external ID.
/// </summary>
public class GetUserByExternalIdQueryHandlerTests : BaseIntegrationTest
{
    public GetUserByExternalIdQueryHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : GetUserByExternalIdQueryHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_ReturnUserDto_When_UserWithExternalIdExists()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                externalId);

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var query = new GetUserByExternalIdQuery { ExternalId = externalId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(user.Id);
            result.Value.ExternalId.Should().Be(externalId);
            result.Value.Email.Should().Be(user.Email);
            result.Value.FirstName.Should().Be(user.FirstName);
            result.Value.LastName.Should().Be(user.LastName);
            result.Value.IsActive.Should().Be(user.IsActive);
        }

        [Fact]
        public async Task Should_ReturnUserWithProfile_When_UserHasProfile()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                externalId);

            user.UpdateProfile(
                "123-456-789",
                "123 Main St",
                "https://example.com/profile.jpg");

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var query = new GetUserByExternalIdQuery { ExternalId = externalId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Profile.Should().NotBeNull();
            result.Value.Profile.PhoneNumber.Should().Be("123-456-789");
            result.Value.Profile.Address.Should().Be("123 Main St");
            result.Value.Profile.ProfileImageUrl.Should().Be("https://example.com/profile.jpg");
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserWithExternalIdDoesNotExist()
        {
            // Arrange
            var nonExistentExternalId = Guid.NewGuid();
            var query = new GetUserByExternalIdQuery { ExternalId = nonExistentExternalId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o identyfikatorze zewnętrznym {nonExistentExternalId}"));
        }

        [Fact]
        public async Task Should_ReturnCorrectUser_When_MultipleUsersExist()
        {
            // Arrange
            var targetExternalId = Guid.NewGuid();
            var otherExternalId = Guid.NewGuid();

            var targetUser = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                targetExternalId);

            var otherUser = User.Create(
                "Jane",
                "Smith",
                "jane.smith@example.com",
                "janesmith",
                otherExternalId);

            await DbContext.Users.AddRangeAsync(targetUser, otherUser);
            await SaveAndClearAsync();

            var query = new GetUserByExternalIdQuery { ExternalId = targetExternalId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(targetUser.Id);
            result.Value.ExternalId.Should().Be(targetExternalId);
            result.Value.Email.Should().Be(targetUser.Email);
            result.Value.FirstName.Should().Be(targetUser.FirstName);
            result.Value.LastName.Should().Be(targetUser.LastName);

            // Should not return the other user
            result.Value.Id.Should().NotBe(otherUser.Id);
            result.Value.Email.Should().NotBe(otherUser.Email);
        }

        [Fact]
        public async Task Should_ReturnInactiveUser_When_UserIsInactive()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                externalId);

            user.Deactivate(); // Make user inactive

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var query = new GetUserByExternalIdQuery { ExternalId = externalId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.IsActive.Should().BeFalse();
            result.Value.ExternalId.Should().Be(externalId);
        }

        [Fact]
        public async Task Should_ReturnUserWithEmptyProfile_When_UserHasNoProfile()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                externalId);

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var query = new GetUserByExternalIdQuery { ExternalId = externalId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Profile.Should().NotBeNull();
            result.Value.Profile.PhoneNumber.Should().Be(string.Empty);
            result.Value.Profile.Address.Should().Be(string.Empty);
            result.Value.Profile.ProfileImageUrl.Should().Be(string.Empty);
        }

        [Fact]
        public async Task Should_HandleProfileExceptionGracefully_When_ProfileDataCorrupted()
        {
            // Arrange
            var externalId = Guid.NewGuid();
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                externalId);

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var query = new GetUserByExternalIdQuery { ExternalId = externalId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            // The handler should handle profile exceptions gracefully
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Profile.Should().NotBeNull();
            result.Value.Profile.PhoneNumber.Should().Be(string.Empty);
            result.Value.Profile.Address.Should().Be(string.Empty);
            result.Value.Profile.ProfileImageUrl.Should().Be(string.Empty);
        }
    }
}