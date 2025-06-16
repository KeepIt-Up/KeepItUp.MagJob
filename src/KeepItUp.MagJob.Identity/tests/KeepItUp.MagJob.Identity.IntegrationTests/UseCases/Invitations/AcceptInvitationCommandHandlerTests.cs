using FluentAssertions;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.AcceptInvitation;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Invitations;

/// <summary>
/// Integration tests for AcceptInvitationCommandHandler.
/// </summary>
public class AcceptInvitationCommandHandlerTests : BaseIntegrationTest
{
    public AcceptInvitationCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Should_AcceptInvitation_When_ValidInvitation()
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

        var command = new AcceptInvitationCommand
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
        updatedInvitation!.Status.Should().Be(InvitationStatus.Accepted);

        // Note: Domain events would be processed in production to add member automatically
        // This test only verifies that the invitation was accepted successfully
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_InvitationDoesNotExist()
    {
        // Arrange
        var nonExistentInvitationId = Guid.NewGuid();
        var command = new AcceptInvitationCommand
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
        result.Errors.Should().Contain(e => e.Contains($"Invitation with ID {nonExistentInvitationId} not found"));
    }

    [Fact]
    public async Task Should_ReturnError_When_UserWithEmailDoesNotExist()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(owner);
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
            "nonexistent@example.com", // Email that doesn't exist in database
            memberRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await DbContext.Invitations.AddAsync(invitation);
        await SaveAndClearAsync();

        var command = new AcceptInvitationCommand
        {
            InvitationId = invitation.Id,
            Token = invitation.Token,
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.Errors.Should().Contain(e => e.Contains("User with email nonexistent@example.com not found"));

        // Verify invitation was still accepted (domain logic happens first)
        var updatedInvitation = await DbContext.Invitations.FindAsync(invitation.Id);
        updatedInvitation!.Status.Should().Be(InvitationStatus.Accepted);
    }

    [Fact]
    public async Task Should_ReturnError_When_OrganizationDoesNotExist()
    {
        // Arrange
        var invitee = User.Create(
            "Invitee",
            "User",
            "invitee@example.com",
            "invitee",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(invitee);
        await SaveAndClearAsync();

        var nonExistentOrganizationId = Guid.NewGuid();
        var invitation = Invitation.Create(
            nonExistentOrganizationId,
            invitee.Email,
            Guid.NewGuid()); // Random role ID

        await DbContext.Invitations.AddAsync(invitation);
        await SaveAndClearAsync();

        var command = new AcceptInvitationCommand
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
        result.Errors.Should().Contain(e => e.Contains($"Organization with ID {nonExistentOrganizationId} not found"));

        // Verify invitation was accepted
        var updatedInvitation = await DbContext.Invitations.FindAsync(invitation.Id);
        updatedInvitation!.Status.Should().Be(InvitationStatus.Accepted);
    }

    [Fact]
    public async Task Should_ReturnError_When_InvitationIsAlreadyAccepted()
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

        invitation.Accept(invitation.Token);

        await DbContext.Organizations.AddAsync(organization);
        await DbContext.Invitations.AddAsync(invitation);
        await SaveAndClearAsync();

        var command = new AcceptInvitationCommand
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
        result.Errors.Should().Contain(e => e.Contains("Tylko oczekujące zaproszenia mogą zostać zaakceptowane"));
    }

    [Fact]
    public async Task Should_ReturnError_When_InvitationIsExpired()
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
            memberRole.Id,
            DateTime.UtcNow.AddDays(-1)); // Expired yesterday

        await DbContext.Organizations.AddAsync(organization);
        await DbContext.Invitations.AddAsync(invitation);
        await SaveAndClearAsync();

        var command = new AcceptInvitationCommand
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
        result.Errors.Should().Contain(e => e.Contains("Nie można zaakceptować wygasłego zaproszenia"));
    }

    // Note: Test for member creation failure removed since handler no longer checks member existence
    // Member creation is now handled by InvitationAcceptedEventHandler via domain events

    [Fact]
    public async Task Should_AcceptInvitation_When_TokenMatches()
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

        // Use correct token for successful acceptance
        var command = new AcceptInvitationCommand
        {
            InvitationId = invitation.Id,
            Token = invitation.Token, // Use correct token
            UserId = invitee.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        // Verify invitation status
        var updatedInvitation = await DbContext.Invitations.FindAsync(invitation.Id);
        updatedInvitation!.Status.Should().Be(InvitationStatus.Accepted);

        // Note: Domain events would be processed in production to add member automatically
        // This test only verifies that the invitation was accepted successfully
    }

    [Fact]
    public async Task Should_ReturnError_When_TokenIsInvalid()
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

        var command = new AcceptInvitationCommand
        {
            InvitationId = invitation.Id,
            Token = "wrong-token", // Invalid token
            UserId = invitee.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.Errors.Should().Contain(e => e.Contains("Nieprawidłowy token zaproszenia"));

        // Verify invitation was NOT accepted
        var updatedInvitation = await DbContext.Invitations.FindAsync(invitation.Id);
        updatedInvitation!.Status.Should().Be(InvitationStatus.Pending);
    }
}