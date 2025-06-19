using FluentAssertions;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.RejectInvitation;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Invitations;

/// <summary>
/// Integration tests for RejectInvitationCommandHandler.
/// </summary>
public class RejectInvitationCommandHandlerTests : BaseIntegrationTest
{
    public RejectInvitationCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Should_RejectInvitation_When_ValidInvitation()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var invitee = User.Create(
            "Invitee",
            "User",
            "invitee@example.com",
            "invitee",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, invitee);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var invitation = Invitation.Create(
            organization.Id,
            invitee.Email,
            memberRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await DbContext.Invitations.AddAsync(invitation);
        await SaveAndClearAsync();

        var command = new RejectInvitationCommand
        {
            InvitationId = invitation.Id,
            Token = invitation.Token,
            UserId = invitee.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify invitation status
        var updatedInvitation = await DbContext.Invitations.FindAsync(invitation.Id);
        updatedInvitation.Should().NotBeNull();
        updatedInvitation!.Status.Should().Be(InvitationStatus.Rejected);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_InvitationDoesNotExist()
    {
        // Arrange
        var nonExistentInvitationId = Guid.NewGuid();
        var command = new RejectInvitationCommand
        {
            InvitationId = nonExistentInvitationId,
            Token = "some-token",
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono zaproszenia o ID {nonExistentInvitationId}"));
    }

    [Fact]
    public async Task Should_ReturnError_When_InvitationIsAlreadyRejected()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var invitee = User.Create(
            "Invitee",
            "User",
            "invitee@example.com",
            "invitee",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, invitee);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var invitation = Invitation.Create(
            organization.Id,
            invitee.Email,
            memberRole.Id);

        invitation.Reject();

        await DbContext.Organizations.AddAsync(organization);
        await DbContext.Invitations.AddAsync(invitation);
        await SaveAndClearAsync();

        var command = new RejectInvitationCommand
        {
            InvitationId = invitation.Id,
            Token = invitation.Token,
            UserId = invitee.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.Errors.Should().Contain(e => e.Contains("Tylko oczekujące zaproszenia mogą zostać odrzucone"));

        // Verify invitation status remains rejected
        var updatedInvitation = await DbContext.Invitations.FindAsync(invitation.Id);
        updatedInvitation.Should().NotBeNull();
        updatedInvitation!.Status.Should().Be(InvitationStatus.Rejected);
    }
}