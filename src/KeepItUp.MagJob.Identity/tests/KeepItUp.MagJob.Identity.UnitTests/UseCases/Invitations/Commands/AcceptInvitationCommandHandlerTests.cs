using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.AcceptInvitation;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Invitations.Commands;

/// <summary>
/// Tests for AcceptInvitationCommandHandler.
/// </summary>
public class AcceptInvitationCommandHandlerTests : BaseUnitTest
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AcceptInvitationCommandHandler> _logger;
    private readonly AcceptInvitationCommandHandler _handler;

    public AcceptInvitationCommandHandlerTests()
    {
        _invitationRepository = RepositoryMockFactory.CreateSuccessfulInvitationRepository();
        _organizationRepository = RepositoryMockFactory.CreateSuccessfulOrganizationRepository();
        _userRepository = RepositoryMockFactory.CreateSuccessfulUserRepository();
        _logger = MockFactory.CreateLogger<AcceptInvitationCommandHandler>();
        _handler = new AcceptInvitationCommandHandler(_invitationRepository, _organizationRepository, _userRepository, _logger);
    }

    public class Handle : AcceptInvitationCommandHandlerTests
    {
        [Fact]
        public async Task Should_AcceptInvitation_When_AllEntitiesExistAndMemberExists()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var memberRole = organization.AddRole("Member", "Member role");
            var invitation = InvitationMother.InvitationForOrganization(organization.Id);
            var user = UserMother.UserWithEmail(invitation.Email);

            // Add member to organization (simulating that event handler already processed)
            var member = organization.AddMember(user.Id, memberRole.Id);

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify invitation was accepted
            invitation.Status.Should().Be(InvitationStatus.Accepted);

            // Verify repository interactions
            await _invitationRepository.Received(1).GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>());
            await _invitationRepository.Received(1).UpdateAsync(invitation, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_InvitationDoesNotExist()
        {
            // Arrange
            var invitationId = GenerateId();
            var command = new AcceptInvitationCommand
            {
                InvitationId = invitationId,
                Token = "some-token",
                UserId = GenerateId()
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns((Invitation?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Invitation with ID {invitationId} not found"));

            // Verify no further operations were performed
            await _invitationRepository.DidNotReceive().UpdateAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
            await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_UserDoesNotExist()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = GenerateId()
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains($"User with email {invitation.Email} not found"));

            // Verify invitation was still accepted (domain logic)
            invitation.Status.Should().Be(InvitationStatus.Accepted);
            await _invitationRepository.Received(1).UpdateAsync(invitation, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_OrganizationDoesNotExist()
        {
            // Arrange
            var user = UserMother.ValidUser();
            var invitation = InvitationMother.ValidInvitation();

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains($"Organization with ID {invitation.OrganizationId} not found"));

            // Verify invitation was accepted and user was found
            invitation.Status.Should().Be(InvitationStatus.Accepted);
            await _invitationRepository.Received(1).UpdateAsync(invitation, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_AcceptInvitation_When_MemberWillBeCreatedAsynchronously()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var invitation = InvitationMother.InvitationForOrganization(organization.Id);
            var user = UserMother.UserWithEmail(invitation.Email);

            // Note: Member creation is handled by event handler asynchronously, so we don't add member here

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify all steps were performed
            invitation.Status.Should().Be(InvitationStatus.Accepted);
            await _invitationRepository.Received(1).UpdateAsync(invitation, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_InvitationAcceptThrowsException()
        {
            // Arrange - Create already accepted invitation
            var invitation = InvitationMother.AcceptedInvitation();

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = GenerateId()
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas akceptowania zaproszenia"));
            result.Errors.Should().Contain(e => e.Contains("Tylko oczekujące zaproszenia mogą zostać zaakceptowane"));

            // Verify no update was attempted
            await _invitationRepository.DidNotReceive().UpdateAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_InvitationIsExpired()
        {
            // Arrange
            var invitation = InvitationMother.ExpiredInvitation();

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = GenerateId()
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas akceptowania zaproszenia"));
            result.Errors.Should().Contain(e => e.Contains("wygasłego zaproszenia"));

            // Verify no update was attempted
            await _invitationRepository.DidNotReceive().UpdateAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_GetByIdAsyncThrowsException()
        {
            // Arrange
            var invitationId = GenerateId();
            var command = new AcceptInvitationCommand
            {
                InvitationId = invitationId,
                Token = "some-token",
                UserId = GenerateId()
            };

            var exception = new InvalidOperationException("Database connection failed");
            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Invitation?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas akceptowania zaproszenia"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));

            await _invitationRepository.DidNotReceive().UpdateAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_LogSuccess_When_InvitationIsAcceptedSuccessfully()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var adminRole = organization.AddRole("Admin", "Admin role");
            var invitation = InvitationMother.InvitationForOrganization(organization.Id);
            var user = UserMother.UserWithEmail(invitation.Email);
            var member = organization.AddMember(user.Id, adminRole.Id);

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>())
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
            var guestRole = organization.AddRole("Guest", "Guest role");
            var invitation = InvitationMother.InvitationForOrganization(organization.Id);
            var user = UserMother.UserWithEmail(invitation.Email);
            var member = organization.AddMember(user.Id, guestRole.Id);

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };
            var cancellationToken = new CancellationToken();

            _invitationRepository.GetByIdAsync(command.InvitationId, cancellationToken)
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, cancellationToken)
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, cancellationToken)
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify the exact cancellation token was passed to all operations
            await _invitationRepository.Received(1).GetByIdAsync(command.InvitationId, cancellationToken);
            await _invitationRepository.Received(1).UpdateAsync(invitation, cancellationToken);
            await _userRepository.Received(1).GetByEmailAsync(invitation.Email, cancellationToken);
            await _organizationRepository.Received(1).GetByIdAsync(invitation.OrganizationId, cancellationToken);
        }

        [Fact]
        public async Task Should_CallCorrectRepositoryMethods_When_HandlingCommand()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var memberRole = organization.AddRole("Member", "Member role");
            var invitation = InvitationMother.InvitationForOrganization(organization.Id);
            var user = UserMother.UserWithEmail(invitation.Email);
            var member = organization.AddMember(user.Id, memberRole.Id);

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify that GetByIdAsync was called
            await _organizationRepository.Received(1).GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>());

            // Verify GetByEmailAsync was called on user repository
            await _userRepository.Received(1).GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>());
            await _userRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_AcceptInvitationAndEmitDomainEvent_When_ValidCommand()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var memberRole = organization.AddRole("Member", "Member role");
            var invitation = InvitationMother.InvitationForOrganization(organization.Id);
            var user = UserMother.UserWithEmail(invitation.Email);
            var member = organization.AddMember(user.Id, memberRole.Id);

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(invitation.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify invitation status changed
            invitation.Status.Should().Be(InvitationStatus.Accepted);

            // Verify domain event was emitted
            invitation.DomainEvents.Should().NotBeEmpty();
            invitation.DomainEvents.Should().Contain(e => e.GetType().Name == "InvitationAcceptedEvent");
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test.email+tag@domain.co.uk")]
        [InlineData("very.long.email.address@very.long.domain.name.com")]
        public async Task Should_AcceptInvitation_When_EmailHasVariousFormats(string email)
        {
            // Arrange
            var user = UserMother.UserWithEmail(email);
            var organization = OrganizationMother.ValidOrganization();
            var adminRole = organization.AddRole("Admin", "Admin role");
            var invitation = Invitation.Create(organization.Id, email, adminRole.Id);
            var member = organization.AddMember(user.Id, adminRole.Id);

            var command = new AcceptInvitationCommand
            {
                InvitationId = invitation.Id,
                Token = invitation.Token,
                UserId = user.Id
            };

            _invitationRepository.GetByIdAsync(command.InvitationId, Arg.Any<CancellationToken>())
                .Returns(invitation);

            _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>())
                .Returns(user);

            _organizationRepository.GetByIdAsync(invitation.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            invitation.Status.Should().Be(InvitationStatus.Accepted);
            invitation.Email.Should().Be(email);

            await _userRepository.Received(1).GetByEmailAsync(email, Arg.Any<CancellationToken>());
        }
    }
}
