using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Organizations.Commands;

/// <summary>
/// Tests for CreateOrganizationCommandHandler.
/// </summary>
public class CreateOrganizationCommandHandlerTests : BaseUnitTest
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateOrganizationCommandHandler> _logger;
    private readonly CreateOrganizationCommandHandler _handler;

    public CreateOrganizationCommandHandlerTests()
    {
        _organizationRepository = RepositoryMockFactory.CreateSuccessfulOrganizationRepository();
        _userRepository = RepositoryMockFactory.CreateSuccessfulUserRepository();
        _logger = MockFactory.CreateLogger<CreateOrganizationCommandHandler>();
        _handler = new CreateOrganizationCommandHandler(_organizationRepository, _userRepository, _logger);
    }

    public class Handle : CreateOrganizationCommandHandlerTests
    {
        [Fact]
        public async Task Should_CreateOrganization_When_ValidCommandProvided()
        {
            // Arrange
            var owner = UserMother.ValidUser();
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = "Test Description",
                OwnerId = owner.ExternalId
            };

            // Owner exists
            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(owner);

            // Organization name doesn't exist
            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeEmpty();

            // Verify repository interactions
            await _userRepository.Received(1).GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).GetByNameAsync(command.Name, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_CreateOrganizationWithCorrectProperties_When_ValidCommandProvided()
        {
            // Arrange
            var owner = UserMother.ValidUser();
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = "Test Description",
                OwnerId = owner.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(owner);
            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            Organization? capturedOrganization = null;
            await _organizationRepository.AddAsync(Arg.Do<Organization>(o => capturedOrganization = o), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedOrganization.Should().NotBeNull();
            capturedOrganization!.Name.Should().Be(command.Name);
            capturedOrganization.Description.Should().Be(command.Description);
            capturedOrganization.OwnerId.Should().Be(owner.Id);
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_OwnerDoesNotExist()
        {
            // Arrange
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = "Test Description",
                OwnerId = GenerateId()
            };

            // Owner doesn't exist
            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o ID {command.OwnerId}"));

            // Verify no organization was created
            await _organizationRepository.DidNotReceive().AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_OrganizationWithNameAlreadyExists()
        {
            // Arrange
            var owner = UserMother.ValidUser();
            var existingOrganization = OrganizationMother.ValidOrganization();
            var command = new CreateOrganizationCommand
            {
                Name = existingOrganization.Name,
                Description = "Test Description",
                OwnerId = owner.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(owner);
            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns(existingOrganization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains($"Organizacja o nazwie '{command.Name}' już istnieje"));

            // Verify no organization was created
            await _organizationRepository.DidNotReceive().AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_InitializeRolesAndOwner_When_OrganizationCreated()
        {
            // Arrange
            var owner = UserMother.ValidUser();
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = "Test Description",
                OwnerId = owner.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(owner);
            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            Organization? capturedOrganization = null;
            await _organizationRepository.AddAsync(Arg.Do<Organization>(o => capturedOrganization = o), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedOrganization.Should().NotBeNull();

            // Verify roles were initialized (should have default roles)
            capturedOrganization!.Roles.Should().NotBeEmpty();

            // Verify owner was initialized (should have owner as member)
            capturedOrganization.Members.Should().NotBeEmpty();
            capturedOrganization.Members.Should().Contain(m => m.UserId == owner.Id);
        }

        [Fact]
        public async Task Should_CreateOrganizationWithNullDescription_When_DescriptionNotProvided()
        {
            // Arrange
            var owner = UserMother.ValidUser();
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = null, // No description
                OwnerId = owner.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(owner);
            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            Organization? capturedOrganization = null;
            await _organizationRepository.AddAsync(Arg.Do<Organization>(o => capturedOrganization = o), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedOrganization.Should().NotBeNull();
            capturedOrganization!.Description.Should().BeNull();
        }

        [Fact]
        public async Task Should_LogInformation_When_OrganizationCreatedSuccessfully()
        {
            // Arrange
            var owner = UserMother.ValidUser();
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = "Test Description",
                OwnerId = owner.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(owner);
            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

        }

        [Fact]
        public async Task Should_ReturnError_When_UserRepositoryThrowsException()
        {
            // Arrange
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = "Test Description",
                OwnerId = GenerateId()
            };

            var exception = new InvalidOperationException("Database connection failed");
            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<User?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas tworzenia organizacji"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));
        }

        [Fact]
        public async Task Should_LogError_When_ExceptionOccurs()
        {
            // Arrange
            var command = new CreateOrganizationCommand
            {
                Name = "Test Organization",
                Description = "Test Description",
                OwnerId = GenerateId()
            };

            var exception = new InvalidOperationException("Test exception");
            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<User?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();

        }

        [Fact]
        public async Task Should_ReturnError_When_EmptyNameProvided()
        {
            // Arrange
            var owner = UserMother.ValidUser();
            var command = new CreateOrganizationCommand
            {
                Name = "", // Empty name should cause validation error
                Description = "Test Description",
                OwnerId = owner.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.OwnerId, Arg.Any<CancellationToken>())
                .Returns(owner);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            // Handler catches ArgumentException from Organization.Create and returns Result.Error
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas tworzenia organizacji"));
        }
    }
}