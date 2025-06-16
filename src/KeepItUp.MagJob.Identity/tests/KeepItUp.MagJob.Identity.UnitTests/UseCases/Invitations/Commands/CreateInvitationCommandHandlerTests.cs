using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.CreateInvitation;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Invitations.Commands;

/// <summary>
/// Tests for CreateInvitationCommandHandler.
/// </summary>
public class CreateInvitationCommandHandlerTests : BaseUnitTest
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateInvitationCommandHandler> _logger;
    private readonly CreateInvitationCommandHandler _handler;

    public CreateInvitationCommandHandlerTests()
    {
        _invitationRepository = RepositoryMockFactory.CreateSuccessfulInvitationRepository();
        _organizationRepository = RepositoryMockFactory.CreateSuccessfulOrganizationRepository();
        _userRepository = RepositoryMockFactory.CreateSuccessfulUserRepository();
        _logger = MockFactory.CreateLogger<CreateInvitationCommandHandler>();
        _handler = new CreateInvitationCommandHandler(_invitationRepository, _organizationRepository, _userRepository, _logger);
    }

    public class Handle : CreateInvitationCommandHandlerTests
    {
        [Fact]
        public async Task Should_CreateInvitation_When_OrganizationAndRoleExist()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var adminRole = organization.AddRole("Admin", "Administrator role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, adminRole.Id); // User has access to organization

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = "test@example.com",
                RoleId = adminRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);

            // Verify repository interactions
            await _userRepository.Received(1).GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>());
            await _invitationRepository.Received(1).HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>());
            await _invitationRepository.Received(1).AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_OrganizationDoesNotExist()
        {
            // Arrange
            var organizationId = GenerateId();
            var roleId = GenerateId();
            var user = UserMother.ValidUser();
            var command = new CreateInvitationCommand
            {
                OrganizationId = organizationId,
                Email = "test@example.com",
                RoleId = roleId,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono organizacji o ID {organizationId}"));

            // Verify no invitation was created
            await _invitationRepository.DidNotReceive().AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_RoleDoesNotExistInOrganization()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var user = UserMother.ValidUser();
            var adminRole = organization.AddRole("Admin", "Admin role");
            organization.AddMember(user.Id, adminRole.Id); // User has access
            var nonExistentRoleId = GenerateId(); // Role that doesn't exist in organization

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = "test@example.com",
                RoleId = nonExistentRoleId,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono roli o ID {nonExistentRoleId} w organizacji {organization.Id}"));

            // Verify no invitation was created
            await _invitationRepository.DidNotReceive().AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_CreateInvitationWithCorrectData_When_ValidCommand()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var memberRole = organization.AddRole("Member", "Member role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, memberRole.Id); // User has access
            var email = "john.doe@example.com";

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = email,
                RoleId = memberRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            Invitation? capturedInvitation = null;
            await _invitationRepository.AddAsync(Arg.Do<Invitation>(inv => capturedInvitation = inv), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            capturedInvitation.Should().NotBeNull();
            capturedInvitation!.OrganizationId.Should().Be(organization.Id);
            capturedInvitation.Email.Should().Be(email);
            capturedInvitation.RoleId.Should().Be(memberRole.Id);
            capturedInvitation.Status.Should().Be(InvitationStatus.Pending);
            capturedInvitation.Id.Should().NotBe(Guid.Empty);

            result.Value.Should().Be(capturedInvitation.Id);
        }

        [Fact]
        public async Task Should_ReturnError_When_GetByIdWithRolesAsyncThrowsException()
        {
            // Arrange
            var organizationId = GenerateId();
            var user = UserMother.ValidUser();
            var command = new CreateInvitationCommand
            {
                OrganizationId = organizationId,
                Email = "test@example.com",
                RoleId = GenerateId(),
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            var exception = new InvalidOperationException("Database connection failed");
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Organization?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas tworzenia zaproszenia"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));

            await _invitationRepository.DidNotReceive().AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_AddAsyncThrowsException()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var adminRole = organization.AddRole("Admin", "Administrator role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, adminRole.Id); // User has access

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = "test@example.com",
                RoleId = adminRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            var exception = new InvalidOperationException("Failed to save invitation");
            _invitationRepository.AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Invitation>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas tworzenia zaproszenia"));
            result.Errors.Should().Contain(e => e.Contains("Failed to save invitation"));
        }

        [Fact]
        public async Task Should_LogSuccess_When_InvitationIsCreated()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var guestRole = organization.AddRole("Guest", "Guest role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, guestRole.Id); // User has access
            var email = "guest@example.com";

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = email,
                RoleId = guestRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);

            // Verify logging
            _logger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString()!.Contains("Utworzono zaproszenie")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task Should_UseCorrectCancellationToken_When_Provided()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var memberRole = organization.AddRole("Member", "Member role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, memberRole.Id); // User has access
            var cancellationToken = new CancellationToken(true);

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = "test@example.com",
                RoleId = memberRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify that correct cancellation token was used
            await _userRepository.Received(1).GetByExternalIdAsync(command.UserId, cancellationToken);
            await _organizationRepository.Received(1).GetByIdWithRolesAsync(command.OrganizationId, cancellationToken);
            await _invitationRepository.Received(1).HasPendingInvitationAsync(command.OrganizationId, command.Email, cancellationToken);
            await _invitationRepository.Received(1).AddAsync(Arg.Any<Invitation>(), cancellationToken);
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test.email+tag@domain.co.uk")]
        [InlineData("very.long.email.address@very.long.domain.name.com")]
        public async Task Should_CreateInvitation_When_EmailHasVariousFormats(string email)
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var memberRole = organization.AddRole("Member", "Member role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, memberRole.Id); // User has access

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = email,
                RoleId = memberRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_CreateInvitationWithPendingStatus_When_ValidCommand()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var adminRole = organization.AddRole("Admin", "Administrator role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, adminRole.Id); // User has access
            var email = "admin@example.com";

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = email,
                RoleId = adminRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            Invitation? capturedInvitation = null;
            await _invitationRepository.AddAsync(Arg.Do<Invitation>(inv => capturedInvitation = inv), Arg.Any<CancellationToken>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            capturedInvitation.Should().NotBeNull();
            capturedInvitation!.Status.Should().Be(InvitationStatus.Pending);
        }

        [Fact]
        public async Task Should_CallCorrectRepositoryMethods_When_HandlingCommand()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var memberRole = organization.AddRole("Member", "Member role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, memberRole.Id); // User has access

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = "test@example.com",
                RoleId = memberRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify repository method calls
            await _userRepository.Received(1).GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>());
            await _invitationRepository.Received(1).HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>());
            await _invitationRepository.Received(1).AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_InvitationCreateThrowsException()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var guestRole = organization.AddRole("Guest", "Guest role");
            var user = UserMother.ValidUser();
            organization.AddMember(user.Id, guestRole.Id); // User has access

            var command = new CreateInvitationCommand
            {
                OrganizationId = organization.Id,
                Email = "test@example.com",
                RoleId = guestRole.Id,
                UserId = user.ExternalId
            };

            _userRepository.GetByExternalIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);
            _organizationRepository.GetByIdWithRolesAsync(command.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);
            _invitationRepository.HasPendingInvitationAsync(command.OrganizationId, command.Email, Arg.Any<CancellationToken>())
                .Returns(false);

            var exception = new InvalidOperationException("Invitation creation failed");
            _invitationRepository.AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Invitation>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas tworzenia zaproszenia"));
            result.Errors.Should().Contain(e => e.Contains("Invitation creation failed"));
        }
    }
}