using FluentAssertions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for RevokeRoleFromMemberCommandHandler.
/// </summary>
public class RevokeRoleFromMemberCommandHandlerTests : BaseIntegrationTest
{
    public RevokeRoleFromMemberCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Should_RevokeRoleFromMember_When_ValidRequest()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var member = User.Create(
            "Member",
            "User",
            "member@example.com",
            "member",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, member);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        // Add member with Member role first, then assign Admin role
        var memberRole = organization.Roles.First(r => r.Name == "Member");
        var adminRole = organization.Roles.First(r => r.Name == "Admin");

        organization.AddMember(member.Id, memberRole.Id);
        // Add admin role to the member (so they have both Member and Admin roles)
        var organizationMember = organization.Members.First(m => m.UserId == member.Id);
        organizationMember.AssignRole(adminRole.Id, adminRole);

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = organization.Id,
            MemberUserId = member.Id,
            RoleId = adminRole.Id,
            RequestingUserId = owner.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify the role was revoked in the MemberRoles table
        var memberEntity = await DbContext.Members
            .FirstOrDefaultAsync(m => m.UserId == member.Id && m.OrganizationId == organization.Id);

        memberEntity.Should().NotBeNull();

        // Check if the role was revoked by loading the member with roles
        var updatedMember = await DbContext.Members
            .Include(m => m.Roles)
            .FirstOrDefaultAsync(m => m.UserId == member.Id && m.OrganizationId == organization.Id);

        updatedMember.Should().NotBeNull();
        updatedMember!.Roles.Should().NotContain(r => r.Id == adminRole.Id);
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

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = Guid.NewGuid(),
            MemberUserId = user.Id,
            RoleId = Guid.NewGuid(),
            RequestingUserId = user.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains("Nie znaleziono organizacji"));
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_RoleDoesNotExist()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var member = User.Create(
            "Member",
            "User",
            "member@example.com",
            "member",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, member);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        // Add member as a regular member
        var memberRole = organization.Roles.First(r => r.Name == "Member");
        organization.AddMember(member.Id, memberRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = organization.Id,
            MemberUserId = member.Id,
            RoleId = Guid.NewGuid(),
            RequestingUserId = owner.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains("Nie znaleziono roli"));
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_MemberIsNotInOrganization()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var nonMember = User.Create(
            "NonMember",
            "User",
            "nonmember@example.com",
            "nonmember",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, nonMember);
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

        // Get the Admin role to revoke
        var adminRole = organization.Roles.First(r => r.Name == "Admin");

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = organization.Id,
            MemberUserId = nonMember.Id,
            RoleId = adminRole.Id,
            RequestingUserId = owner.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains("nie jest członkiem organizacji"));
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_RequestingUserIsNotOwnerOrAdmin()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var adminMember = User.Create(
            "Admin",
            "User",
            "admin@example.com",
            "admin",
            Guid.NewGuid());

        var regularMember = User.Create(
            "Regular",
            "User",
            "regular@example.com",
            "regular",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, adminMember, regularMember);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        // Add admin member with Admin role
        var adminRole = organization.Roles.First(r => r.Name == "Admin");
        organization.AddMember(adminMember.Id, adminRole.Id);

        // Add regular member with Member role
        var memberRole = organization.Roles.First(r => r.Name == "Member");
        organization.AddMember(regularMember.Id, memberRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = organization.Id,
            MemberUserId = adminMember.Id,
            RoleId = adminRole.Id,
            RequestingUserId = regularMember.Id // Regular member trying to revoke role
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
        result.Errors.Should().Contain(e => e.Contains("Brak uprawnień"));
    }

    [Fact]
    public async Task Should_ReturnError_When_MemberDoesNotHaveRole()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var member = User.Create(
            "Member",
            "User",
            "member@example.com",
            "member",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, member);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        // Add member with Member role only
        var memberRole = organization.Roles.First(r => r.Name == "Member");
        organization.AddMember(member.Id, memberRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        // Try to revoke Admin role which the member doesn't have
        var adminRole = organization.Roles.First(r => r.Name == "Admin");
        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = organization.Id,
            MemberUserId = member.Id,
            RoleId = adminRole.Id,
            RequestingUserId = owner.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.Errors.Should().Contain(e => e.Contains("nie ma przypisanej roli"));
    }

    [Fact]
    public async Task Should_ReturnError_When_AttemptingToRevokeOwnerRole()
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

        // Get the Admin role to try to revoke from the owner  
        var adminRole = organization.Roles.First(r => r.Name == "Admin");

        var command = new RevokeRoleFromMemberCommand
        {
            OrganizationId = organization.Id,
            MemberUserId = owner.Id,
            RoleId = adminRole.Id,
            RequestingUserId = owner.Id
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Error);
        result.Errors.Should().Contain(e => e.Contains("Nie można odebrać roli właściciela"));
    }
}