using FluentAssertions;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.CreateInvitation;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Invitations;

public class CreateInvitationCommandHandlerTests : BaseIntegrationTest
{
    public CreateInvitationCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Should_CreateInvitation_When_ValidDataProvided()
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

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var command = new CreateInvitationCommand
        {
            OrganizationId = organization.Id,
            Email = "invitee@example.com",
            RoleId = memberRole.Id,
            UserId = owner.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var invitation = await DbContext.Invitations.FirstOrDefaultAsync(i => i.Id == result.Value);
        invitation.Should().NotBeNull();
        invitation!.OrganizationId.Should().Be(organization.Id);
        invitation.Email.Should().Be(command.Email);
        invitation.RoleId.Should().Be(memberRole.Id);
        invitation.Status.Should().Be(InvitationStatus.Pending);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_OrganizationDoesNotExist()
    {
        // Arrange
        var user = User.Create(
            "User",
            "Test",
            "user@example.com",
            "user",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(user);
        await SaveAndClearAsync();

        var nonExistentOrganizationId = Guid.NewGuid();
        var command = new CreateInvitationCommand
        {
            OrganizationId = nonExistentOrganizationId,
            Email = "invitee@example.com",
            RoleId = Guid.NewGuid(),
            UserId = user.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono organizacji o ID {nonExistentOrganizationId}"));
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_UserDoesNotExist()
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

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var nonExistentUserId = Guid.NewGuid();
        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var command = new CreateInvitationCommand
        {
            OrganizationId = organization.Id,
            Email = "invitee@example.com",
            RoleId = memberRole.Id,
            UserId = nonExistentUserId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono użytkownika o ID {nonExistentUserId}"));
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_UserIsNotAuthorizedToInvite()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var regularUser = User.Create(
            "Regular",
            "User",
            "regular@example.com",
            "regular",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, regularUser);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var command = new CreateInvitationCommand
        {
            OrganizationId = organization.Id,
            Email = "invitee@example.com",
            RoleId = memberRole.Id,
            UserId = regularUser.ExternalId // User who is not a member
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
        result.Errors.Should().Contain(e => e.Contains("Brak uprawnień do zapraszania użytkowników"));
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_RoleDoesNotExistInOrganization()
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

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var nonExistentRoleId = Guid.NewGuid();
        var command = new CreateInvitationCommand
        {
            OrganizationId = organization.Id,
            Email = "invitee@example.com",
            RoleId = nonExistentRoleId,
            UserId = owner.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono roli o ID {nonExistentRoleId}"));
    }

    [Fact]
    public async Task Should_ReturnError_When_UserAlreadyInvited()
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

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var existingInvitation = Invitation.Create(
            organization.Id,
            "invitee@example.com",
            memberRole.Id);

        await DbContext.Invitations.AddAsync(existingInvitation);
        await SaveAndClearAsync();

        var command = new CreateInvitationCommand
        {
            OrganizationId = organization.Id,
            Email = "invitee@example.com", // Same email as existing invitation
            RoleId = memberRole.Id,
            UserId = owner.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("już istnieje"));
    }

    [Fact]
    public async Task Should_CreateInvitation_When_UserHasAdminRole()
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

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var command = new CreateInvitationCommand
        {
            OrganizationId = organization.Id,
            Email = "invitee@example.com",
            RoleId = memberRole.Id,
            UserId = owner.ExternalId // Owner creating invitation (has Admin privileges)
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var invitation = await DbContext.Invitations.FirstOrDefaultAsync(i => i.Id == result.Value);
        invitation.Should().NotBeNull();
        invitation!.OrganizationId.Should().Be(organization.Id);
    }

    [Fact]
    public async Task Should_GenerateInvitationToken_When_InvitationCreated()
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

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var command = new CreateInvitationCommand
        {
            OrganizationId = organization.Id,
            Email = "invitee@example.com",
            RoleId = memberRole.Id,
            UserId = owner.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var invitation = await DbContext.Invitations.FirstOrDefaultAsync(i => i.Id == result.Value);
        invitation.Should().NotBeNull();
        invitation!.Token.Should().NotBeNullOrEmpty();
        invitation.Token.Length.Should().BeGreaterThan(10); // Should be a proper token
        invitation.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}