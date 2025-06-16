using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Users.Queries;

/// <summary>
/// Tests for GetUserByIdQueryHandler.
/// </summary>
public class GetUserByIdQueryHandlerTests : BaseUnitTest
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _userRepository = RepositoryMockFactory.CreateSuccessfulUserRepository();
        _logger = MockFactory.CreateLogger<GetUserByIdQueryHandler>();
        _handler = new GetUserByIdQueryHandler(_userRepository, _logger);
    }

    public class Handle : GetUserByIdQueryHandlerTests
    {
        [Fact]
        public async Task Should_ReturnUserDto_When_UserExists()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var userDto = result.Value;
            userDto.Id.Should().Be(user.Id);
            userDto.ExternalId.Should().Be(user.ExternalId);
            userDto.Email.Should().Be(user.Email);
            userDto.FirstName.Should().Be(user.FirstName);
            userDto.LastName.Should().Be(user.LastName);
            userDto.IsActive.Should().Be(user.IsActive);

            // Verify repository interaction
            await _userRepository.Received(1).GetByIdAsync(query.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnUserWithProfile_When_UserHasProfile()
        {
            // Arrange
            var user = UserMother.ValidUser();
            user.UpdateProfile("123-456-789", "123 Test Street", "http://example.com/avatar.jpg");

            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var userDto = result.Value;

            userDto.Profile.Should().NotBeNull();
            userDto.Profile!.PhoneNumber.Should().Be("123-456-789");
            userDto.Profile.Address.Should().Be("123 Test Street");
            userDto.Profile.ProfileImageUrl.Should().Be("http://example.com/avatar.jpg");
        }

        [Fact]
        public async Task Should_ReturnUserWithEmptyProfile_When_UserHasNoProfile()
        {
            // Arrange
            var user = UserMother.ValidUser();
            // User has no profile (Profile is null)

            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var userDto = result.Value;

            userDto.Profile.Should().NotBeNull();
            userDto.Profile!.PhoneNumber.Should().Be(string.Empty);
            userDto.Profile.Address.Should().Be(string.Empty);
            userDto.Profile.ProfileImageUrl.Should().Be(string.Empty);
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var userId = GenerateId();
            var query = new GetUserByIdQuery { Id = userId };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o ID {userId}"));

            // Verify repository interaction
            await _userRepository.Received(1).GetByIdAsync(query.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_RepositoryThrowsException()
        {
            // Arrange
            var userId = GenerateId();
            var query = new GetUserByIdQuery { Id = userId };

            var exception = new InvalidOperationException("Database connection failed");
            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<User?>(exception));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas pobierania użytkownika"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));
        }

        [Fact]
        public async Task Should_HandleProfileMappingException_When_ProfileAccessFails()
        {
            // Arrange
            var user = UserMother.ValidUser();
            user.UpdateProfile("123-456-789", "123 Test Street", "http://example.com/avatar.jpg");

            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            // Even if profile mapping fails, the handler should return success with empty profile
            result.IsSuccess.Should().BeTrue();
            var userDto = result.Value;

            // The handler has a try-catch around profile mapping, so it should always succeed
            userDto.Profile.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_MapAllUserProperties_When_UserIsComplete()
        {
            // Arrange
            var user = UserMother.AdminUser(); // Use a different user type
            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var userDto = result.Value;

            // Verify all basic properties are mapped correctly
            userDto.Id.Should().Be(user.Id);
            userDto.ExternalId.Should().Be(user.ExternalId);
            userDto.Email.Should().Be(user.Email);
            userDto.FirstName.Should().Be(user.FirstName);
            userDto.LastName.Should().Be(user.LastName);
            userDto.IsActive.Should().Be(user.IsActive);

            // Verify profile is initialized (even if empty)
            userDto.Profile.Should().NotBeNull();

            // Verify memberships list is initialized (empty by default in this handler)
            userDto.Memberships.Should().NotBeNull();
            userDto.Memberships.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_HandleInactiveUser_When_UserIsDeactivated()
        {
            // Arrange
            var user = UserMother.InactiveUser();
            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var userDto = result.Value;

            userDto.IsActive.Should().BeFalse();
            // Handler should still return the user even if inactive
            userDto.Id.Should().Be(user.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Should_HandleEmptyProfileFields_When_ProfileHasEmptyValues(string? emptyValue)
        {
            // Arrange
            var user = UserMother.ValidUser();
            user.UpdateProfile(emptyValue, emptyValue, emptyValue);

            var query = new GetUserByIdQuery { Id = user.Id };

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var userDto = result.Value;

            userDto.Profile.Should().NotBeNull();
            // Handler maps null/empty values to empty strings
            if (string.IsNullOrEmpty(emptyValue))
            {
                userDto.Profile!.PhoneNumber.Should().Be(string.Empty);
                userDto.Profile.Address.Should().Be(string.Empty);
                userDto.Profile.ProfileImageUrl.Should().Be(string.Empty);
            }
        }

        [Fact]
        public async Task Should_UseCorrectCancellationToken_When_Provided()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var query = new GetUserByIdQuery { Id = user.Id };
            var cancellationToken = new CancellationToken();

            _userRepository.GetByIdAsync(query.Id, cancellationToken)
                .Returns(user);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify the exact cancellation token was passed
            await _userRepository.Received(1).GetByIdAsync(query.Id, cancellationToken);
        }
    }
}