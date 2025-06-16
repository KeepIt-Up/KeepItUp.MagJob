using FluentAssertions;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

public class GetOrganizationByIdQueryHandlerTests : BaseIntegrationTest
{
    public GetOrganizationByIdQueryHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Should_ReturnOrganization_When_UserIsMemberAndOrganizationExists()
    {
        // Arrange
        var user = User.Create(
            "John",
            "Doe",
            "john.doe@example.com",
            "johndoe",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(user);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            user.Id,
            "Test description",
            "logo.jpg",
            "banner.jpg");

        organization.InitializeRoles();
        organization.InitializeOwner();

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = organization.Id,
            UserId = user.Id
        };

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(organization.Id);
        result.Value.Name.Should().Be(organization.Name);
        result.Value.Description.Should().Be(organization.Description);
        result.Value.LogoUrl.Should().Be(organization.LogoUrl);
        result.Value.BannerUrl.Should().Be(organization.BannerUrl);
        result.Value.IsActive.Should().Be(organization.IsActive);
        result.Value.OwnerId.Should().Be(organization.OwnerId);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_OrganizationDoesNotExist()
    {
        // Arrange
        var user = User.Create(
            "John",
            "Doe",
            "john.doe@example.com",
            "johndoe",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(user);
        await SaveAndClearAsync();

        var nonExistentOrganizationId = Guid.NewGuid();
        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = nonExistentOrganizationId,
            UserId = user.Id
        };

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono organizacji o ID {nonExistentOrganizationId}"));
    }

    [Fact]
    public async Task Should_ReturnOrganization_When_UserIsNotMember()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var nonMemberUser = User.Create(
            "NonMember",
            "User",
            "nonmember@example.com",
            "nonmember",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, nonMemberUser);
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

        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = organization.Id,
            UserId = nonMemberUser.Id // User who is not a member
        };

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue(); // No authorization implemented currently
        result.Value.Should().NotBeNull();
        result.Value.UserRoles.Should().BeEmpty(); // Non-member has no roles
    }

    [Fact]
    public async Task Should_ReturnOrganization_When_UserDoesNotExist()
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

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var nonExistentUserId = Guid.NewGuid();
        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = organization.Id,
            UserId = nonExistentUserId
        };

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue(); // No user validation implemented currently
        result.Value.Should().NotBeNull();
        result.Value.UserRoles.Should().BeEmpty(); // Non-existent user has no roles
    }

    [Fact]
    public async Task Should_ReturnOrganization_When_UserIsOwner()
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

        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = organization.Id,
            UserId = owner.Id
        };

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(organization.Id);
        result.Value.OwnerId.Should().Be(owner.Id);
    }

    [Fact]
    public async Task Should_ReturnOrganizationWithNullValues_When_OptionalFieldsAreNull()
    {
        // Arrange
        var user = User.Create(
            "John",
            "Doe",
            "john.doe@example.com",
            "johndoe",
            Guid.NewGuid());

        await DbContext.Users.AddAsync(user);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            user.Id,
            null, // No description
            null, // No logo
            null); // No banner

        organization.InitializeRoles();
        organization.InitializeOwner();

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = organization.Id,
            UserId = user.Id
        };

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Description.Should().BeNull();
        result.Value.LogoUrl.Should().BeNull();
        result.Value.BannerUrl.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnOrganization_When_UserHasAdminRole()
    {
        // Arrange
        var owner = User.Create(
            "Owner",
            "User",
            "owner@example.com",
            "owner",
            Guid.NewGuid());

        var admin = User.Create(
            "Admin",
            "User",
            "admin@example.com",
            "admin",
            Guid.NewGuid());

        await DbContext.Users.AddRangeAsync(owner, admin);
        await SaveAndClearAsync();

        var organization = Organization.Create(
            "Test Organization",
            owner.Id,
            "Test description",
            null,
            null);

        organization.InitializeRoles();
        organization.InitializeOwner();

        // Add admin as a member with Admin role
        var adminRole = organization.Roles.First(r => r.Name == "Admin");
        organization.AddMember(admin.Id, adminRole.Id);

        await DbContext.Organizations.AddAsync(organization);
        await SaveAndClearAsync();

        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = organization.Id,
            UserId = admin.Id
        };

        // Act
        var result = await Mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(organization.Id);
    }
}
