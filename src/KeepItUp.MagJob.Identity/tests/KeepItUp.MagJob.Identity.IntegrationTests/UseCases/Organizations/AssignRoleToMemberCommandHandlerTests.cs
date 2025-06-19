using FluentAssertions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for AssignRoleToMemberCommandHandler.
/// </summary>
public class AssignRoleToMemberCommandHandlerTests : BaseIntegrationTest
{
    public AssignRoleToMemberCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Should_AssignRoleToMember_When_ValidRequest()
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

        // Get the Admin role to assign
        var adminRole = await DbContext.Roles
            .FirstAsync(r => r.OrganizationId == organization.Id && r.Name == "Admin");

        var command = new AssignRoleToMemberCommand
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

        // Verify the role was assigned in the MemberRoles table
        var memberEntity = await DbContext.Members
            .Include(m => m.Roles)
            .FirstOrDefaultAsync(m => m.UserId == member.Id && m.OrganizationId == organization.Id);

        memberEntity.Should().NotBeNull();

        // Check if the role was assigned
        memberEntity!.Roles.Should().Contain(r => r.Id == adminRole.Id);
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

        var command = new AssignRoleToMemberCommand
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

        var command = new AssignRoleToMemberCommand
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

        // Get the Admin role to assign
        var adminRole = organization.Roles.First(r => r.Name == "Admin");

        var command = new AssignRoleToMemberCommand
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

        var member1 = User.Create(
            "Member1",
            "User",
            "member1@example.com",
            "member1",
            Guid.NewGuid());

        var member2 = User.Create(
            "Member2",
            "User",
            "member2@example.com",
            "member2",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, member1, member2);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        // Add both users as regular members
        var memberRole = organization.Roles.First(r => r.Name == "Member");
        organization.AddMember(member1.Id, memberRole.Id);
        organization.AddMember(member2.Id, memberRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        // Get the Admin role to assign
        var adminRole = organization.Roles.First(r => r.Name == "Admin");

        var command = new AssignRoleToMemberCommand
        {
            OrganizationId = organization.Id,
            MemberUserId = member1.Id,
            RoleId = adminRole.Id,
            RequestingUserId = member2.Id // Regular member trying to assign role
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Forbidden);
        result.Errors.Should().Contain(e => e.Contains("Brak uprawnień"));
    }

    [Fact]
    public async Task Should_ReturnError_When_MemberAlreadyHasRole()
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

        // Add member with admin role already
        var adminRole = organization.Roles.First(r => r.Name == "Admin");
        organization.AddMember(member.Id, adminRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var command = new AssignRoleToMemberCommand
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
        result.Errors.Should().Contain(e => e.Contains("już ma przypisaną rolę"));
    }
}