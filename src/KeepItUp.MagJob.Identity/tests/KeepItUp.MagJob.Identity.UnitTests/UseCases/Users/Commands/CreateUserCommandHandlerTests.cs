using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Users.Commands;

/// <summary>
/// Tests for CreateUserCommandHandler.
/// </summary>
public class CreateUserCommandHandlerTests : BaseUnitTest
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepository = RepositoryMockFactory.CreateSuccessfulUserRepository();
        _logger = MockFactory.CreateLogger<CreateUserCommandHandler>();
        _handler = new CreateUserCommandHandler(_userRepository, _logger);
    }

    public class Handle : CreateUserCommandHandlerTests
    {
        [Fact]
        public async Task Should_CreateUser_When_ValidCommandProvided()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = GenerateId(),
                Email = GenerateEmail(),
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe"
            };

            // User doesn't exist
            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);
            _userRepository.GetByExternalIdAsync(command.ExternalId, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            // Verify repository interactions
            await _userRepository.Received(1).GetByEmailAsync(command.Email, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).GetByExternalIdAsync(command.ExternalId, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_UserWithEmailAlreadyExists()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = GenerateId(),
                Email = GenerateEmail(),
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe"
            };

            var existingUser = User.Create("Jane", "Smith", command.Email, "janesmith", GenerateId());
            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("adresie e-mail już istnieje"));

            // Verify no user was added
            await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_UserWithExternalIdAlreadyExists()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = GenerateId(),
                Email = GenerateEmail(),
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe"
            };

            var existingUser = User.Create("Jane", "Smith", "jane@example.com", "janesmith", command.ExternalId);

            // Email doesn't exist, but ExternalId does
            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);
            _userRepository.GetByExternalIdAsync(command.ExternalId, Arg.Any<CancellationToken>())
                .Returns(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("identyfikatorze zewnętrznym już istnieje"));

            // Verify no user was added
            await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_CreateUserWithProfile_When_ProfileDataProvided()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = GenerateId(),
                Email = GenerateEmail(),
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123-456-789",
                Address = "123 Test Street",
                ProfileImageUrl = "http://example.com/avatar.jpg"
            };

            // User doesn't exist
            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);
            _userRepository.GetByExternalIdAsync(command.ExternalId, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            User? capturedUser = null;
            await _userRepository.AddAsync(Arg.Do<User>(u => capturedUser = u), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedUser.Should().NotBeNull();
            capturedUser!.Profile.Should().NotBeNull();
            capturedUser.Profile!.PhoneNumber.Should().Be(command.PhoneNumber);
            capturedUser.Profile.Address.Should().Be(command.Address);
            capturedUser.Profile.ProfileImage.Should().Be(command.ProfileImageUrl);
        }

        [Fact]
        public async Task Should_UseEmailAsUsername_When_UsernameIsEmpty()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                ExternalId = GenerateId(),
                Email = GenerateEmail(),
                Username = "", // Empty username
                FirstName = "John",
                LastName = "Doe"
            };

            // User doesn't exist
            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);
            _userRepository.GetByExternalIdAsync(command.ExternalId, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            User? capturedUser = null;
            await _userRepository.AddAsync(Arg.Do<User>(u => capturedUser = u), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedUser.Should().NotBeNull();
            capturedUser!.Username.Should().Be(command.Email);
        }
    }
}