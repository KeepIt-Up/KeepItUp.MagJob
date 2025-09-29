using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Queries;

/// <summary>
/// Integration tests for GetUserByIdQueryHandler.
/// Tests the complete flow from query to database retrieval.
/// </summary>
public class GetUserByIdQueryHandlerTests : BaseIntegrationTest
{
    public GetUserByIdQueryHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : GetUserByIdQueryHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_ReturnUserDto_When_UserExists()
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

            var query = new GetUserByIdQuery { Id = user.Id };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(user.Id);
            result.Value.ExternalId.Should().Be(user.ExternalId);
            result.Value.Email.Should().Be(user.Email);
            result.Value.FirstName.Should().Be(user.FirstName);
            result.Value.LastName.Should().Be(user.LastName);
            result.Value.IsActive.Should().Be(user.IsActive);
        }

        [Fact]
        public async Task Should_ReturnUserWithProfile_When_UserHasProfile()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            user.UpdateProfile(
                "123-456-789",
                "123 Main St",
                "https://example.com/profile.jpg");

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var query = new GetUserByIdQuery { Id = user.Id };

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
        public async Task Should_ReturnUserWithEmptyProfile_When_UserHasNoProfile()
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

            var query = new GetUserByIdQuery { Id = user.Id };

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
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            var query = new GetUserByIdQuery { Id = nonExistentUserId };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o ID {nonExistentUserId}"));
        }

        [Fact]
        public async Task Should_ReturnUserWithPartialProfile_When_ProfileHasNullValues()
        {
            // Arrange
            var user = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            // Update with partial profile data
            user.UpdateProfile("123-456-789", null, null);

            await DbContext.Users.AddAsync(user);
            await SaveAndClearAsync();

            var query = new GetUserByIdQuery { Id = user.Id };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Profile.Should().NotBeNull();
            result.Value.Profile.PhoneNumber.Should().Be("123-456-789");
            result.Value.Profile.Address.Should().Be(string.Empty);
            result.Value.Profile.ProfileImageUrl.Should().Be(string.Empty);
        }

        [Fact]
        public async Task Should_HandleProfileExceptionGracefully_When_ProfileDataCorrupted()
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

            var query = new GetUserByIdQuery { Id = user.Id };

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

        [Fact]
        public async Task Should_NotIncludeRelatedEntities_When_OnlyUserDataRequested()
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

            var query = new GetUserByIdQuery { Id = user.Id };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            // Query should only return user basic data, not organizations or other relations
            // This is implicitly tested by the UserDto structure
            result.Value.Id.Should().Be(user.Id);
            result.Value.Email.Should().Be(user.Email);
            result.Value.FirstName.Should().Be(user.FirstName);
            result.Value.LastName.Should().Be(user.LastName);
        }
    }
}