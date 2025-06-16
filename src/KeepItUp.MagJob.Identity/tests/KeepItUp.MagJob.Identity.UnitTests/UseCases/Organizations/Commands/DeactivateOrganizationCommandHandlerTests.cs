using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Organizations.Commands;

/// <summary>
/// Tests for DeactivateOrganizationCommandHandler.
/// </summary>
public class DeactivateOrganizationCommandHandlerTests : BaseUnitTest
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<DeactivateOrganizationCommandHandler> _logger;
    private readonly DeactivateOrganizationCommandHandler _handler;

    public DeactivateOrganizationCommandHandlerTests()
    {
        _organizationRepository = RepositoryMockFactory.CreateSuccessfulOrganizationRepository();
        _logger = MockFactory.CreateLogger<DeactivateOrganizationCommandHandler>();
        _handler = new DeactivateOrganizationCommandHandler(_organizationRepository, _logger);
    }

    public class Handle : DeactivateOrganizationCommandHandlerTests
    {
        [Fact]
        public async Task Should_DeactivateOrganization_When_UserIsOwnerAndOrganizationIsActive()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId // User is the owner
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify organization was deactivated
            organization.IsActive.Should().BeFalse();

            // Verify repository interactions
            await _organizationRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnForbidden_When_UserIsNotOwner()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var nonOwnerUserId = GenerateId(); // Different user, not the owner

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = nonOwnerUserId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Forbidden);
            result.Errors.Should().Contain(e => e.Contains("Tylko właściciel organizacji może ją dezaktywować"));

            // Verify organization was not deactivated
            organization.IsActive.Should().BeTrue();

            // Verify no update was attempted
            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_OrganizationDoesNotExist()
        {
            // Arrange
            var organizationId = GenerateId();
            var userId = GenerateId();
            var command = new DeactivateOrganizationCommand
            {
                Id = organizationId,
                UserId = userId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono organizacji o ID {organizationId}"));

            // Verify no update was attempted
            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_DeactivateOrganization_When_OrganizationIsAlreadyInactive()
        {
            // Arrange - Test idempotent behavior
            var organization = OrganizationMother.InactiveOrganization();
            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify organization remains inactive (idempotent)
            organization.IsActive.Should().BeFalse();

            // Verify repository interactions still occurred
            await _organizationRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_GetByIdAsyncThrowsException()
        {
            // Arrange
            var organizationId = GenerateId();
            var userId = GenerateId();
            var command = new DeactivateOrganizationCommand
            {
                Id = organizationId,
                UserId = userId
            };

            var exception = new InvalidOperationException("Database connection failed");
            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Organization?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas dezaktywacji organizacji"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));

            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_UpdateAsyncThrowsException()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            var exception = new InvalidOperationException("Update failed");
            _organizationRepository.UpdateAsync(organization, Arg.Any<CancellationToken>())
                .Returns(Task.FromException(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas dezaktywacji organizacji"));
            result.Errors.Should().Contain(e => e.Contains("Update failed"));
        }

        [Fact]
        public async Task Should_LogSuccess_When_DeactivationCompletes()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify logging occurred
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
            var organization = OrganizationMother.ValidOrganization();
            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };
            var cancellationToken = new CancellationToken();

            _organizationRepository.GetByIdAsync(command.Id, cancellationToken)
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify the exact cancellation token was passed to all operations
            await _organizationRepository.Received(1).GetByIdAsync(command.Id, cancellationToken);
            await _organizationRepository.Received(1).UpdateAsync(organization, cancellationToken);
        }

        [Fact]
        public async Task Should_EmitDomainEvent_When_OrganizationIsDeactivated()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization was deactivated
            organization.IsActive.Should().BeFalse();

            // Verify domain event was emitted
            organization.DomainEvents.Should().NotBeEmpty();
            organization.DomainEvents.Should().Contain(e => e.GetType().Name == "OrganizationDeactivatedEvent");
        }

        [Fact]
        public async Task Should_PreserveOrganizationData_When_Deactivated()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var originalName = organization.Name;
            var originalDescription = organization.Description;
            var originalOwnerId = organization.OwnerId;

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization data is preserved
            organization.IsActive.Should().BeFalse();
            organization.Name.Should().Be(originalName);
            organization.Description.Should().Be(originalDescription);
            organization.OwnerId.Should().Be(originalOwnerId);
            organization.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_CallCorrectRepositoryMethods_When_HandlingCommand()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify that GetByIdAsync was called (not GetByIdWithMembersAsync or other variants)
            await _organizationRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
            await _organizationRepository.DidNotReceive().GetByIdWithMembersAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
            await _organizationRepository.DidNotReceive().GetByIdWithRolesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_DeactivateOrganization_When_OrganizationHasDifferentActiveStates(bool initialActiveState)
        {
            // Arrange
            var organization = initialActiveState
                ? OrganizationMother.ValidOrganization()
                : OrganizationMother.InactiveOrganization();

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization is deactivated regardless of initial state
            organization.IsActive.Should().BeFalse();

            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }
    }
}