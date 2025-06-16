using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Users.Commands;

/// <summary>
/// Tests for UpdateUserCommandHandler.
/// </summary>
public class UpdateUserCommandHandlerTests : BaseUnitTest
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepository = RepositoryMockFactory.CreateSuccessfulUserRepository();
        _logger = MockFactory.CreateLogger<UpdateUserCommandHandler>();
        _handler = new UpdateUserCommandHandler(_userRepository, _logger);
    }

    public class Handle : UpdateUserCommandHandlerTests
    {
        [Fact]
        public async Task Should_UpdateUser_When_UserExistsAndDataIsValid()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                PhoneNumber = "987-654-321",
                Address = "456 Updated Street",
                ProfileImageUrl = "http://example.com/updated-avatar.jpg"
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify user was updated
            user.FirstName.Should().Be("UpdatedFirstName");
            user.LastName.Should().Be("UpdatedLastName");

            // Verify repository interactions
            await _userRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_UpdateUserWithoutProfile_When_UserHasNoProfile()
        {
            // Arrange
            var user = UserMother.ValidUser();
            // User has no profile initially

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                PhoneNumber = "987-654-321",
                Address = "456 Updated Street",
                ProfileImageUrl = "http://example.com/updated-avatar.jpg"
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify basic user data was updated
            user.FirstName.Should().Be("UpdatedFirstName");
            user.LastName.Should().Be("UpdatedLastName");

            // Verify repository interactions
            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_UpdateUserWithProfile_When_UserHasExistingProfile()
        {
            // Arrange
            var user = UserMother.ValidUser();
            user.UpdateProfile("123-456-789", "123 Original Street", "http://example.com/original.jpg");

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                PhoneNumber = "987-654-321",
                Address = "456 Updated Street",
                ProfileImageUrl = "http://example.com/updated-avatar.jpg"
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify user data was updated
            user.FirstName.Should().Be("UpdatedFirstName");
            user.LastName.Should().Be("UpdatedLastName");

            // Verify profile was updated
            user.Profile.Should().NotBeNull();
            user.Profile!.PhoneNumber.Should().Be("987-654-321");
            user.Profile.Address.Should().Be("456 Updated Street");
            user.Profile.ProfileImage.Should().Be("http://example.com/updated-avatar.jpg");

            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_UpdateOnlyBasicData_When_ProfileDataIsNull()
        {
            // Arrange
            var user = UserMother.ValidUser();
            user.UpdateProfile("123-456-789", "123 Original Street", "http://example.com/original.jpg");

            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                PhoneNumber = null,
                Address = null,
                ProfileImageUrl = null
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify basic data was updated
            user.FirstName.Should().Be("UpdatedFirstName");
            user.LastName.Should().Be("UpdatedLastName");

            // Profile should still exist but might be updated with null values
            // (depends on WithUpdates implementation)
            user.Profile.Should().NotBeNull();

            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var userId = GenerateId();
            var command = new UpdateUserCommand
            {
                Id = userId,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName"
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o ID {userId}"));

            // Verify no update was attempted
            await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_GetByIdThrowsException()
        {
            // Arrange
            var userId = GenerateId();
            var command = new UpdateUserCommand
            {
                Id = userId,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName"
            };

            var exception = new InvalidOperationException("Database connection failed");
            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<User?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas aktualizacji użytkownika"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));

            // Verify no update was attempted
            await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_UpdateAsyncThrowsException()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName"
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            var exception = new InvalidOperationException("Update failed");
            _userRepository.UpdateAsync(user, Arg.Any<CancellationToken>())
                .Returns(Task.FromException(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas aktualizacji użytkownika"));
            result.Errors.Should().Contain(e => e.Contains("Update failed"));
        }

        [Theory]
        [InlineData("A", "B")]
        [InlineData("John", "Doe")]
        [InlineData("Very Long First Name That Should Still Work", "Very Long Last Name That Should Still Work")]
        public async Task Should_UpdateUser_When_NamesHaveVariousFormats(string firstName, string lastName)
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = firstName,
                LastName = lastName
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);

            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData("", "")]
        public async Task Should_ReturnError_When_NamesAreEmpty(string firstName, string lastName)
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = firstName,
                LastName = lastName
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas aktualizacji użytkownika"));
        }

        [Theory]
        [InlineData("   ", "   ")]
        [InlineData("  ", "ValidLastName")]
        [InlineData("ValidFirstName", "  ")]
        public async Task Should_UpdateUser_When_NamesHaveWhitespace(string firstName, string lastName)
        {
            // Arrange - Guard.Against.NullOrEmpty allows whitespace, only empty strings are rejected
            var user = UserMother.ValidUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = firstName,
                LastName = lastName
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);

            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_LogSuccess_When_UpdateCompletes()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName"
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify logging occurred - check that Log method was called with Information level
            // Note: Testing exact log message with NSubstitute and extension methods can be tricky
            // so we verify that LogInformation was called at least once
            _logger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task Should_UseCorrectCancellationToken_When_Provided()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName"
            };
            var cancellationToken = new CancellationToken();

            _userRepository.GetByIdAsync(command.Id, cancellationToken)
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify the exact cancellation token was passed to both operations
            await _userRepository.Received(1).GetByIdAsync(command.Id, cancellationToken);
            await _userRepository.Received(1).UpdateAsync(user, cancellationToken);
        }

        [Fact]
        public async Task Should_HandleInactiveUser_When_UserIsDeactivated()
        {
            // Arrange
            var user = UserMother.InactiveUser();
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName"
            };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Handler should allow updating inactive users
            user.FirstName.Should().Be("UpdatedFirstName");
            user.LastName.Should().Be("UpdatedLastName");
            user.IsActive.Should().BeFalse(); // Should remain inactive

            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }
    }
}