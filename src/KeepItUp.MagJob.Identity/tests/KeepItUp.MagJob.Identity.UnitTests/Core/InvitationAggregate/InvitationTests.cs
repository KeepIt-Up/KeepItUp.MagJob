using FluentAssertions;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Events;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.InvitationAggregate;

/// <summary>
/// Unit tests for Invitation aggregate.
/// Tests core business logic and domain rules.
/// </summary>
public class InvitationTests
{
    /// <summary>
    /// Tests for Invitation creation.
    /// </summary>
    public class Create
    {
        [Fact]
        public void Should_CreateInvitation_When_ValidDataProvided()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var email = "invited@example.com";
            var roleId = Guid.NewGuid();
            var expiresAt = DateTime.UtcNow.AddDays(7);

            // Act
            var invitation = Invitation.Create(organizationId, email, roleId, expiresAt);

            // Assert
            invitation.Should().NotBeNull();
            invitation.OrganizationId.Should().Be(organizationId);
            invitation.Email.Should().Be(email);
            invitation.RoleId.Should().Be(roleId);
            invitation.ExpiresAt.Should().Be(expiresAt);
            invitation.Status.Should().Be(InvitationStatus.Pending);
            invitation.Token.Should().NotBeNullOrEmpty();
            invitation.IsExpired.Should().BeFalse();
        }

        [Fact]
        public void Should_CreateInvitationWithDefaultExpiration_When_NoExpirationProvided()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var email = "invited@example.com";
            var roleId = Guid.NewGuid();

            // Act
            var invitation = Invitation.Create(organizationId, email, roleId);

            // Assert
            invitation.Should().NotBeNull();
            invitation.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void Should_EmitInvitationCreatedEvent_When_InvitationCreated()
        {
            // Arrange & Act
            var invitation = InvitationMother.ValidInvitation();

            // Assert
            invitation.DomainEvents.Should().NotBeEmpty();
            invitation.DomainEvents.Should().Contain(e => e is InvitationCreatedEvent);
        }

        [Fact]
        public void Should_ThrowArgumentException_When_OrganizationIdIsEmpty()
        {
            // Arrange
            var organizationId = Guid.Empty;
            var email = "invited@example.com";
            var roleId = Guid.NewGuid();

            // Act & Assert
            var action = () => Invitation.Create(organizationId, email, roleId);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_ThrowArgumentException_When_EmailIsEmpty()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            // Act & Assert
            var action = () => Invitation.Create(organizationId, "", roleId);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_ThrowArgumentException_When_RoleIdIsEmpty()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var email = "invited@example.com";
            var roleId = Guid.Empty;

            // Act & Assert
            var action = () => Invitation.Create(organizationId, email, roleId);
            action.Should().Throw<ArgumentException>();
        }
    }

    /// <summary>
    /// Tests for Invitation acceptance.
    /// </summary>
    public class Accept
    {
        [Fact]
        public void Should_AcceptInvitation_When_InvitationIsPending()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            // Act
            invitation.Accept();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Accepted);
        }

        [Fact]
        public void Should_EmitInvitationAcceptedEvent_When_InvitationAccepted()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            // Act
            invitation.Accept();

            // Assert
            invitation.DomainEvents.Should().Contain(e => e is InvitationAcceptedEvent);
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_InvitationAlreadyAccepted()
        {
            // Arrange
            var invitation = InvitationMother.AcceptedInvitation();

            // Act & Assert
            var action = () => invitation.Accept();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Tylko oczekujące zaproszenia mogą zostać zaakceptowane.");
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_InvitationIsRejected()
        {
            // Arrange
            var invitation = InvitationMother.RejectedInvitation();

            // Act & Assert
            var action = () => invitation.Accept();
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_InvitationIsExpired()
        {
            // Arrange
            var invitation = InvitationMother.ExpiredInvitation();

            // Act & Assert
            var action = () => invitation.Accept();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Nie można zaakceptować wygasłego zaproszenia.");
        }
    }

    /// <summary>
    /// Tests for Invitation rejection.
    /// </summary>
    public class Reject
    {
        [Fact]
        public void Should_RejectInvitation_When_InvitationIsPending()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            // Act
            invitation.Reject();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Rejected);
        }

        [Fact]
        public void Should_EmitInvitationRejectedEvent_When_InvitationRejected()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            // Act
            invitation.Reject();

            // Assert
            invitation.DomainEvents.Should().Contain(e => e is InvitationRejectedEvent);
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_InvitationAlreadyRejected()
        {
            // Arrange
            var invitation = InvitationMother.RejectedInvitation();

            // Act & Assert
            var action = () => invitation.Reject();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Tylko oczekujące zaproszenia mogą zostać odrzucone.");
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_InvitationIsAccepted()
        {
            // Arrange
            var invitation = InvitationMother.AcceptedInvitation();

            // Act & Assert
            var action = () => invitation.Reject();
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_ThrowInvalidOperationException_When_InvitationIsExpired()
        {
            // Arrange
            var invitation = InvitationMother.ExpiredInvitation();

            // Act & Assert
            var action = () => invitation.Reject();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Nie można odrzucić wygasłego zaproszenia.");
        }
    }

    /// <summary>
    /// Tests for Invitation expiration.
    /// </summary>
    public class Expiration
    {
        [Fact]
        public void Should_MarkAsExpired_When_InvitationIsPending()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            // Act
            invitation.MarkAsExpired();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Expired);
        }

        [Fact]
        public void Should_EmitInvitationExpiredEvent_When_InvitationMarkedAsExpired()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            // Act
            invitation.MarkAsExpired();

            // Assert
            invitation.DomainEvents.Should().Contain(e => e is InvitationExpiredEvent);
        }

        [Fact]
        public void Should_NotChangeStatus_When_InvitationAlreadyAccepted()
        {
            // Arrange
            var invitation = InvitationMother.AcceptedInvitation();

            // Act
            invitation.MarkAsExpired();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Accepted);
        }

        [Fact]
        public void Should_ReturnTrue_When_InvitationIsExpiredByDate()
        {
            // Arrange
            var invitation = InvitationMother.ExpiredInvitation();

            // Act & Assert
            invitation.IsExpired.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_InvitationIsMarkedAsExpired()
        {
            // Arrange
            var invitation = InvitationMother.ManuallyExpiredInvitation();

            // Act & Assert
            invitation.IsExpired.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_InvitationIsNotExpired()
        {
            // Arrange
            var invitation = InvitationMother.ValidInvitation();

            // Act & Assert
            invitation.IsExpired.Should().BeFalse();
        }
    }

    /// <summary>
    /// Tests for InvitationMother factory methods.
    /// </summary>
    public class InvitationMotherTests
    {
        [Fact]
        public void Should_CreateValidInvitation()
        {
            // Act
            var invitation = InvitationMother.ValidInvitation();

            // Assert
            invitation.Should().NotBeNull();
            invitation.Email.Should().Be("invited@example.com");
            invitation.Status.Should().Be(InvitationStatus.Pending);
            invitation.IsExpired.Should().BeFalse();
        }

        [Fact]
        public void Should_CreateExpiredInvitation()
        {
            // Act
            var invitation = InvitationMother.ExpiredInvitation();

            // Assert
            invitation.IsExpired.Should().BeTrue();
        }

        [Fact]
        public void Should_CreateAcceptedInvitation()
        {
            // Act
            var invitation = InvitationMother.AcceptedInvitation();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Accepted);
        }

        [Fact]
        public void Should_CreateRejectedInvitation()
        {
            // Act
            var invitation = InvitationMother.RejectedInvitation();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Rejected);
        }

        [Fact]
        public void Should_CreateInvitationsWithDifferentStatuses()
        {
            // Act
            var invitations = InvitationMother.InvitationsWithDifferentStatuses();

            // Assert
            invitations.Should().HaveCount(4);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Pending);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Accepted);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Rejected);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Expired);
        }
    }

    /// <summary>
    /// Tests for InvitationBuilder fluent API.
    /// </summary>
    public class InvitationBuilderTests
    {
        [Fact]
        public void Should_BuildInvitationWithFluentAPI()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var email = "fluent@example.com";

            // Act
            var invitation = InvitationBuilder.New()
                .ForOrganization(organizationId)
                .WithEmail(email)
                .WithRole(roleId)
                .ExpiringInDays(14)
                .Build();

            // Assert
            invitation.OrganizationId.Should().Be(organizationId);
            invitation.Email.Should().Be(email);
            invitation.RoleId.Should().Be(roleId);
            invitation.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(14), TimeSpan.FromMinutes(1));
            invitation.Status.Should().Be(InvitationStatus.Pending);
        }

        [Fact]
        public void Should_BuildExpiredInvitation()
        {
            // Act
            var invitation = InvitationBuilder.New()
                .Expired()
                .Build();

            // Assert
            invitation.IsExpired.Should().BeTrue();
        }

        [Fact]
        public void Should_BuildAcceptedInvitation()
        {
            // Act
            var invitation = InvitationBuilder.New()
                .Accepted()
                .Build();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Accepted);
        }

        [Fact]
        public void Should_BuildRejectedInvitation()
        {
            // Act
            var invitation = InvitationBuilder.New()
                .Rejected()
                .Build();

            // Assert
            invitation.Status.Should().Be(InvitationStatus.Rejected);
        }

        [Fact]
        public void Should_BuildMultipleInvitations()
        {
            // Act
            var invitations = InvitationBuilder.New()
                .ForOrganization(Guid.NewGuid())
                .WithRole(Guid.NewGuid())
                .BuildMany(3);

            // Assert
            invitations.Should().HaveCount(3);
            invitations.Select(i => i.Email).Should().OnlyHaveUniqueItems();
            invitations.Should().OnlyContain(i => i.Status == InvitationStatus.Pending);
        }

        [Fact]
        public void Should_BuildInvitationsWithDifferentStatuses()
        {
            // Arrange
            var organizationId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            // Act
            var invitations = InvitationBuilder.BuildWithDifferentStatuses(organizationId, roleId);

            // Assert
            invitations.Should().HaveCount(4);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Pending);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Accepted);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Rejected);
            invitations.Should().Contain(i => i.Status == InvitationStatus.Expired);
        }
    }
}