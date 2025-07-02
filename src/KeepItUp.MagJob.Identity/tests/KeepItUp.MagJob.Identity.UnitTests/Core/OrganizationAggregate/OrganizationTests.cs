using FluentAssertions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

namespace KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;

/// <summary>
/// Unit tests for Organization aggregate.
/// Tests core business logic and domain rules.
/// </summary>
public class OrganizationTests
{
    /// <summary>
    /// Tests for Organization creation.
    /// </summary>
    public class Create
    {
        [Fact]
        public void Should_CreateOrganization_When_ValidDataProvided()
        {
            // Arrange
            var name = "Test Organization";
            var ownerId = Guid.NewGuid();
            var description = "Test description";
            var logoUrl = "https://example.com/logo.png";
            var bannerUrl = "https://example.com/banner.png";

            // Act
            var organization = Organization.Create(name, ownerId, description, logoUrl, bannerUrl);

            // Assert
            organization.Should().NotBeNull();
            organization.Name.Should().Be(name);
            organization.OwnerId.Should().Be(ownerId);
            organization.Description.Should().Be(description);
            organization.LogoUrl.Should().Be(logoUrl);
            organization.BannerUrl.Should().Be(bannerUrl);
            organization.IsActive.Should().BeTrue();
            organization.Members.Should().BeEmpty();
            organization.Roles.Should().BeEmpty();
        }

        [Fact]
        public void Should_CreateMinimalOrganization_When_OnlyRequiredFieldsProvided()
        {
            // Arrange
            var name = "Minimal Org";
            var ownerId = Guid.NewGuid();

            // Act
            var organization = Organization.Create(name, ownerId);

            // Assert
            organization.Should().NotBeNull();
            organization.Name.Should().Be(name);
            organization.OwnerId.Should().Be(ownerId);
            organization.Description.Should().BeNull();
            organization.LogoUrl.Should().BeNull();
            organization.BannerUrl.Should().BeNull();
            organization.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_EmitOrganizationCreatedEvent_When_OrganizationCreated()
        {
            // Arrange & Act
            var organization = OrganizationMother.ValidOrganization();

            // Assert
            organization.DomainEvents.Should().NotBeEmpty();
            organization.DomainEvents.Should().Contain(e => e is OrganizationCreatedEvent);
        }

        [Fact]
        public void Should_ThrowArgumentException_When_NameIsEmpty()
        {
            // Arrange
            var ownerId = Guid.NewGuid();

            // Act & Assert
            var action = () => Organization.Create("", ownerId);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_ThrowArgumentException_When_OwnerIdIsEmpty()
        {
            // Arrange
            var name = "Test Organization";
            var ownerId = Guid.Empty;

            // Act & Assert
            var action = () => Organization.Create(name, ownerId);
            action.Should().Throw<ArgumentException>();
        }
    }

    /// <summary>
    /// Tests for Organization updates.
    /// </summary>
    public class Update
    {
        [Fact]
        public void Should_UpdateOrganization_When_ValidDataProvided()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var newName = "Updated Organization";
            var newDescription = "Updated description";
            var newLogoUrl = "https://example.com/new-logo.png";
            var newBannerUrl = "https://example.com/new-banner.png";

            // Act
            organization.Update(newName, newDescription, newLogoUrl, newBannerUrl);

            // Assert
            organization.Name.Should().Be(newName);
            organization.Description.Should().Be(newDescription);
            organization.LogoUrl.Should().Be(newLogoUrl);
            organization.BannerUrl.Should().Be(newBannerUrl);
        }

        [Fact]
        public void Should_ThrowArgumentException_When_UpdateNameIsEmpty()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();

            // Act & Assert
            var action = () => organization.Update("", "Description");
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_UpdateLogo_When_ValidUrlProvided()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var newLogoUrl = "https://example.com/new-logo.png";

            // Act
            organization.UpdateLogo(newLogoUrl);

            // Assert
            organization.LogoUrl.Should().Be(newLogoUrl);
        }

        [Fact]
        public void Should_UpdateBanner_When_ValidUrlProvided()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var newBannerUrl = "https://example.com/new-banner.png";

            // Act
            organization.UpdateBanner(newBannerUrl);

            // Assert
            organization.BannerUrl.Should().Be(newBannerUrl);
        }
    }

    /// <summary>
    /// Tests for Organization status management.
    /// </summary>
    public class StatusManagement
    {
        [Fact]
        public void Should_DeactivateOrganization_When_OrganizationIsActive()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();

            // Act
            organization.Deactivate();

            // Assert
            organization.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_ActivateOrganization_When_OrganizationIsInactive()
        {
            // Arrange
            var organization = OrganizationMother.InactiveOrganization();

            // Act
            organization.Activate();

            // Assert
            organization.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_NotDeactivate_When_OrganizationAlreadyInactive()
        {
            // Arrange
            var organization = OrganizationMother.InactiveOrganization();
            var initialState = organization.IsActive;

            // Act
            organization.Deactivate();

            // Assert
            organization.IsActive.Should().Be(initialState);
        }
    }

    /// <summary>
    /// Tests for Organization roles management.
    /// </summary>
    public class RoleManagement
    {
        [Fact]
        public void Should_InitializeDefaultRoles_When_CalledAfterCreation()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            // Simulate having ID assigned by database
            typeof(Organization).GetProperty("Id")?.SetValue(organization, Guid.NewGuid());

            // Act
            organization.InitializeRoles();

            // Assert
            organization.Roles.Should().HaveCount(3);
            organization.Roles.Should().Contain(r => r.Name == "Admin");
            organization.Roles.Should().Contain(r => r.Name == "Member");
            organization.Roles.Should().Contain(r => r.Name == "Guest");
        }

        [Fact]
        public void Should_AddCustomRole_When_ValidDataProvided()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();
            var roleName = "Custom Role";
            var description = "Custom role description";
            var color = "#FF5733";

            // Act
            var role = organization.AddRole(roleName, description, color);

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().Be(roleName);
            organization.Roles.Should().Contain(role);
        }

        [Fact]
        public void Should_RemoveRole_When_RoleExists()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();
            var roleToRemove = organization.Roles.First();

            // Act
            organization.RemoveRole(roleToRemove.Id);

            // Assert
            organization.Roles.Should().NotContain(roleToRemove);
        }
    }

    /// <summary>
    /// Tests for Organization member management.
    /// </summary>
    public class MemberManagement
    {
        [Fact]
        public void Should_InitializeOwnerMembership_When_CalledAfterRoleInitialization()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();

            // Act
            organization.InitializeOwner();

            // Assert
            organization.Members.Should().HaveCount(1);
            var ownerMember = organization.Members.First();
            ownerMember.UserId.Should().Be(organization.OwnerId);
            var adminRole = organization.Roles.First(r => r.Name == "Admin");
            ownerMember.HasRole(adminRole.Id).Should().BeTrue();
        }

        [Fact]
        public void Should_AddMember_When_ValidUserAndRoleProvided()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();
            var userId = Guid.NewGuid();
            var memberRole = organization.Roles.First(r => r.Name == "Member");

            // Act
            var member = organization.AddMember(userId, memberRole.Id);

            // Assert
            member.Should().NotBeNull();
            member.UserId.Should().Be(userId);
            organization.Members.Should().Contain(member);
        }

        [Fact]
        public void Should_RemoveMember_When_MemberExists()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();
            var userId = Guid.NewGuid();
            var memberRole = organization.Roles.First(r => r.Name == "Member");
            var member = organization.AddMember(userId, memberRole.Id);

            // Act
            organization.RemoveMember(member.UserId);

            // Assert
            organization.Members.Should().NotContain(member);
        }

        [Fact]
        public void Should_AssignRoleToMember_When_MemberAndRoleExist()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithOwnerMembership();
            var member = organization.Members.First();
            var guestRole = organization.Roles.First(r => r.Name == "Guest");

            // Act
            organization.AssignRoleToMember(member.UserId, guestRole.Id);

            // Assert
            member.HasRole(guestRole.Id).Should().BeTrue();
        }

        [Fact]
        public void Should_RevokeRoleFromMember_When_MemberHasMultipleRoles()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithOwnerMembership();
            var member = organization.Members.First();
            var guestRole = organization.Roles.First(r => r.Name == "Guest");
            var adminRole = organization.Roles.First(r => r.Name == "Admin");

            // First assign additional role so member has multiple roles
            organization.AssignRoleToMember(member.UserId, guestRole.Id);

            // Act
            organization.RevokeRoleFromMember(member.UserId, adminRole.Id);

            // Assert
            member.HasRole(adminRole.Id).Should().BeFalse();
            member.HasRole(guestRole.Id).Should().BeTrue();
        }
    }

    /// <summary>
    /// Tests for Organization access control.
    /// </summary>
    public class AccessControl
    {
        [Fact]
        public void Should_ReturnTrue_When_UserHasAccess()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithOwnerMembership();
            var userId = organization.OwnerId;

            // Act
            var hasAccess = organization.HasAccess(userId);

            // Assert
            hasAccess.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_UserHasNoAccess()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();
            var userId = Guid.NewGuid(); // Random user not in organization

            // Act
            var hasAccess = organization.HasAccess(userId);

            // Assert
            hasAccess.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_RoleExists()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();
            var roleId = organization.Roles.First().Id;

            // Act
            var hasRole = organization.HasRole(roleId);

            // Assert
            hasRole.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_RoleDoesNotExist()
        {
            // Arrange
            var organization = OrganizationMother.OrganizationWithRoles();
            var roleId = Guid.NewGuid(); // Random role ID

            // Act
            var hasRole = organization.HasRole(roleId);

            // Assert
            hasRole.Should().BeFalse();
        }
    }

    /// <summary>
    /// Tests for OrganizationMother factory methods.
    /// </summary>
    public class OrganizationMotherTests
    {
        [Fact]
        public void Should_CreateValidOrganization()
        {
            // Act
            var organization = OrganizationMother.ValidOrganization();

            // Assert
            organization.Should().NotBeNull();
            organization.Name.Should().Be("Acme Corporation");
            organization.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_CreateOrganizationWithRoles()
        {
            // Act
            var organization = OrganizationMother.OrganizationWithRoles();

            // Assert
            organization.Roles.Should().HaveCount(3);
            organization.Roles.Should().Contain(r => r.Name == "Admin");
        }

        [Fact]
        public void Should_CreateOrganizationWithOwnerMembership()
        {
            // Act
            var organization = OrganizationMother.OrganizationWithOwnerMembership();

            // Assert
            organization.Members.Should().HaveCount(1);
            organization.Members.First().UserId.Should().Be(organization.OwnerId);
        }
    }

    /// <summary>
    /// Tests for OrganizationBuilder fluent API.
    /// </summary>
    public class OrganizationBuilderTests
    {
        [Fact]
        public void Should_BuildOrganizationWithFluentAPI()
        {
            // Arrange
            var ownerId = Guid.NewGuid();

            // Act
            var organization = OrganizationBuilder.New()
                .WithName("Fluent Organization")
                .WithOwner(ownerId)
                .WithDescription("Built with fluent API")
                .AsActive()
                .Build();

            // Assert
            organization.Name.Should().Be("Fluent Organization");
            organization.OwnerId.Should().Be(ownerId);
            organization.Description.Should().Be("Built with fluent API");
            organization.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_BuildOrganizationWithRoles()
        {
            // Act
            var organization = OrganizationBuilder.New()
                .WithDefaultRoles()
                .Build();

            // Assert
            organization.Roles.Should().HaveCount(3);
        }

        [Fact]
        public void Should_BuildMultipleOrganizations()
        {
            // Act
            var organizations = OrganizationBuilder.New()
                .WithName("Test Org")
                .BuildMany(3);

            // Assert
            organizations.Should().HaveCount(3);
            organizations.Should().OnlyContain(o => o.Name.StartsWith("Test Org"));
        }
    }
}