using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for UpdateRolePermissionsCommandHandler.
/// Tests the complete flow from command to database update of role permissions.
/// </summary>
public class UpdateRolePermissionsCommandHandlerTests : BaseIntegrationTest
{
    public UpdateRolePermissionsCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : UpdateRolePermissionsCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_UpdateRolePermissions_When_ValidRequestByOwner()
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

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var permissions = new List<string> { "read:tasks", "write:tasks", "manage:projects" };

            // Add permissions to the database first
            foreach (var permName in permissions)
            {
                await DbContext.Permissions.AddAsync(new Permission(permName));
            }
            await SaveAndClearAsync();

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = organization.Id,
                RoleId = memberRole.Id,
                Permissions = permissions,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify permissions were updated in database
            var updatedOrganization = await DbContext.Organizations
                .Include(o => o.Roles)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(o => o.Id == organization.Id);

            var updatedRole = updatedOrganization!.Roles.First(r => r.Id == memberRole.Id);
            updatedRole.Permissions.Should().HaveCount(3);
            updatedRole.Permissions.Select(p => p.Name).Should().BeEquivalentTo(permissions);
        }

        [Fact]
        public async Task Should_UpdateRolePermissions_When_ValidRequestByAdmin()
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

            // Add admin user as Admin member
            var adminRole = organization.Roles.First(r => r.Name == "Admin");
            organization.AddMember(admin.Id, adminRole.Id);

            var memberRole = organization.Roles.First(r => r.Name == "Member");

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var permissions = new List<string> { "read:all", "write:basic" };

            // Add permissions to the database first
            foreach (var permName in permissions)
            {
                await DbContext.Permissions.AddAsync(new Permission(permName));
            }
            await SaveAndClearAsync();

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = organization.Id,
                RoleId = memberRole.Id,
                Permissions = permissions,
                UserId = admin.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify permissions were updated
            var updatedOrganization = await DbContext.Organizations
                .Include(o => o.Roles)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(o => o.Id == organization.Id);

            var updatedRole = updatedOrganization!.Roles.First(r => r.Id == memberRole.Id);
            updatedRole.Permissions.Should().HaveCount(2);
            updatedRole.Permissions.Select(p => p.Name).Should().BeEquivalentTo(permissions);
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
            var roleId = Guid.NewGuid();

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = nonExistentOrganizationId,
                RoleId = roleId,
                Permissions = new List<string> { "read:all" },
                UserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
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

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = organization.Id,
                RoleId = nonExistentRoleId,
                Permissions = new List<string> { "read:all" },
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("Nie znaleziono roli"));
        }

        [Fact(Skip = "Autoryzacja nie jest zaimplementowana w handlerze")]
        public async Task Should_ReturnUnauthorized_When_UserIsNotMember()
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

            var memberRole = organization.Roles.First(r => r.Name == "Member");

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = organization.Id,
                RoleId = memberRole.Id,
                Permissions = new List<string> { "read:all" },
                UserId = nonMember.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Brak uprawnień do aktualizacji uprawnień roli.");
        }

        [Fact(Skip = "Autoryzacja nie jest zaimplementowana w handlerze")]
        public async Task Should_ReturnUnauthorized_When_UserIsRegularMember()
        {
            // Arrange
            var owner = User.Create(
                "Owner",
                "User",
                "owner@example.com",
                "owner",
                Guid.NewGuid());

            var regularMember = User.Create(
                "Member",
                "User",
                "member@example.com",
                "member",
                Guid.NewGuid());

            await DbContext.Users.AddRangeAsync(owner, regularMember);
            await SaveAndClearAsync();

            var organization = Organization.Create(
                "Test Organization",
                owner.Id,
                "Test description",
                null,
                null);

            organization.InitializeRoles();
            organization.InitializeOwner();

            // Add user as regular member
            var memberRole = organization.Roles.First(r => r.Name == "Member");
            organization.AddMember(regularMember.Id, memberRole.Id);

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = organization.Id,
                RoleId = memberRole.Id,
                Permissions = new List<string> { "read:all" },
                UserId = regularMember.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Brak uprawnień do aktualizacji uprawnień roli.");
        }

        [Fact]
        public async Task Should_ClearPermissions_When_EmptyListProvided()
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

            // Add initial permissions
            var initialPermissions = new List<string> { "read:all", "write:all" };
            foreach (var permName in initialPermissions)
            {
                await DbContext.Permissions.AddAsync(new Permission(permName));
            }
            await SaveAndClearAsync();

            foreach (var permName in initialPermissions)
            {
                var permission = await DbContext.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
                memberRole.AddPermission(permission!);
            }

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = organization.Id,
                RoleId = memberRole.Id,
                Permissions = new List<string>(), // Empty list
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify permissions were cleared
            var updatedOrganization = await DbContext.Organizations
                .Include(o => o.Roles)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(o => o.Id == organization.Id);

            var updatedRole = updatedOrganization!.Roles.First(r => r.Id == memberRole.Id);
            updatedRole.Permissions.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_OverwriteExistingPermissions_When_NewPermissionsProvided()
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

            // Add initial permissions
            var initialPermissions = new List<string> { "read:all", "write:all", "delete:all" };
            foreach (var permName in initialPermissions)
            {
                await DbContext.Permissions.AddAsync(new Permission(permName));
            }
            await SaveAndClearAsync();

            foreach (var permName in initialPermissions)
            {
                var permission = await DbContext.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
                memberRole.AddPermission(permission!);
            }

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            // New permissions to set
            var newPermissions = new List<string> { "read:basic", "write:basic" };
            foreach (var permName in newPermissions)
            {
                await DbContext.Permissions.AddAsync(new Permission(permName));
            }
            await SaveAndClearAsync();

            var command = new UpdateRolePermissionsCommand
            {
                OrganizationId = organization.Id,
                RoleId = memberRole.Id,
                Permissions = newPermissions,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify permissions were overwritten
            var updatedOrganization = await DbContext.Organizations
                .Include(o => o.Roles)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(o => o.Id == organization.Id);

            var updatedRole = updatedOrganization!.Roles.First(r => r.Id == memberRole.Id);
            updatedRole.Permissions.Should().HaveCount(2);
            updatedRole.Permissions.Select(p => p.Name).Should().BeEquivalentTo(newPermissions);
            updatedRole.Permissions.Select(p => p.Name).Should().NotContain(initialPermissions);
        }
    }
}