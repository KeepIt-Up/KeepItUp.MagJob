using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.DeactivateUser;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Users.Commands;

/// <summary>
/// Tests for DeactivateUserCommandHandler.
/// </summary>
public class DeactivateUserCommandHandlerTests : BaseUnitTest
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeactivateUserCommandHandler> _logger;
    private readonly DeactivateUserCommandHandler _handler;

    public DeactivateUserCommandHandlerTests()
    {
        _userRepository = RepositoryMockFactory.CreateSuccessfulUserRepository();
        _logger = MockFactory.CreateLogger<DeactivateUserCommandHandler>();
        _handler = new DeactivateUserCommandHandler(_userRepository, _logger);
    }

    public class Handle : DeactivateUserCommandHandlerTests
    {
        [Fact]
        public async Task Should_DeactivateUser_When_UserExistsAndIsActive()
        {
            // Arrange
            var user = UserMother.ValidUser(); // User is active by default
            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify user was deactivated
            user.IsActive.Should().BeFalse();

            // Verify repository interactions
            await _userRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_DeactivateUser_When_UserIsAlreadyInactive()
        {
            // Arrange
            var user = UserMother.InactiveUser(); // User is already inactive
            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // User should remain inactive (idempotent operation)
            user.IsActive.Should().BeFalse();

            // Repository should still be called (handler doesn't check status beforehand)
            await _userRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var userId = GenerateId();
            var command = new DeactivateUserCommand { Id = userId };

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
            var command = new DeactivateUserCommand { Id = userId };

            var exception = new InvalidOperationException("Database connection failed");
            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<User?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas dezaktywacji użytkownika"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));

            // Verify no update was attempted
            await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_UpdateAsyncThrowsException()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new DeactivateUserCommand { Id = user.Id };

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
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas dezaktywacji użytkownika"));
            result.Errors.Should().Contain(e => e.Contains("Update failed"));
        }

        [Fact]
        public async Task Should_LogSuccess_When_DeactivationCompletes()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify logging occurred - check that Log method was called with Information level
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
            var command = new DeactivateUserCommand { Id = user.Id };
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
        public async Task Should_CallDeactivateMethod_When_UserExists()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var originalIsActive = user.IsActive;
            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify the domain method was called (state changed)
            if (originalIsActive)
            {
                user.IsActive.Should().BeFalse(); // Should be deactivated
            }
            else
            {
                user.IsActive.Should().BeFalse(); // Should remain inactive
            }
        }

        [Fact]
        public async Task Should_HandleAdminUser_When_AdminIsDeactivated()
        {
            // Arrange
            var user = UserMother.AdminUser(); // Admin user
            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Even admin users can be deactivated (no special business rules in this handler)
            user.IsActive.Should().BeFalse();

            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnSuccessResult_When_OperationCompletes()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Status.Should().Be(ResultStatus.Ok);
            result.Errors.Should().BeEmpty();

            // Note: DeactivateUserCommand returns Result (not Result<T>)
            // so there's no Value property to check
        }

        [Theory]
        [InlineData(true)]  // Active user
        [InlineData(false)] // Inactive user
        public async Task Should_HandleUserStatus_When_UserHasDifferentInitialStates(bool initialIsActive)
        {
            // Arrange
            var user = initialIsActive ? UserMother.ValidUser() : UserMother.InactiveUser();
            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Regardless of initial state, user should be inactive after deactivation
            user.IsActive.Should().BeFalse();

            await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_PreserveUserData_When_DeactivatingUser()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var originalFirstName = user.FirstName;
            var originalLastName = user.LastName;
            var originalEmail = user.Email;
            var originalExternalId = user.ExternalId;

            var command = new DeactivateUserCommand { Id = user.Id };

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify only IsActive changed, other data preserved
            user.IsActive.Should().BeFalse();
            user.FirstName.Should().Be(originalFirstName);
            user.LastName.Should().Be(originalLastName);
            user.Email.Should().Be(originalEmail);
            user.ExternalId.Should().Be(originalExternalId);
        }
    }
}