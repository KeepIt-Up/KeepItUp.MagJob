using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for DeactivateOrganizationCommandHandler.
/// Tests the complete flow from command to database update of organization activation status.
/// </summary>
public class DeactivateOrganizationCommandHandlerTests : BaseIntegrationTest
{
    public DeactivateOrganizationCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : DeactivateOrganizationCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_DeactivateOrganization_When_ValidRequestByOwner()
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

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization was deactivated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.IsActive.Should().BeFalse();
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

            var command = new DeactivateOrganizationCommand
            {
                Id = nonExistentOrganizationId,
                UserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("Nie znaleziono organizacji"));
        }

        [Fact]
        public async Task Should_ReturnUnauthorized_When_UserIsNotOwner()
        {
            // Arrange
            var owner = User.Create(
                "Owner",
                "User",
                "owner@example.com",
                "owner",
                Guid.NewGuid());

            var nonOwner = User.Create(
                "NonOwner",
                "User",
                "nonowner@example.com",
                "nonowner",
                Guid.NewGuid());

            await DbContext.Users.AddRangeAsync(owner, nonOwner);
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

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = nonOwner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Tylko właściciel organizacji może ją dezaktywować.");
        }

        [Fact]
        public async Task Should_ReturnSuccess_When_OrganizationAlreadyDeactivated()
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
            organization.Deactivate(); // Already deactivated

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization remains deactivated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task Should_AllowAdminToDeactivate_When_UserIsAdminMember()
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

            // Add admin as admin member
            var adminRole = organization.Roles.First(r => r.Name == "Admin");
            organization.AddMember(admin.Id, adminRole.Id);

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = admin.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Tylko właściciel organizacji może ją dezaktywować.");
        }

        [Fact]
        public async Task Should_ReturnUnauthorized_When_UserIsRegularMember()
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

            // Add member as regular member
            var memberRole = organization.Roles.First(r => r.Name == "Member");
            organization.AddMember(member.Id, memberRole.Id);

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = member.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Tylko właściciel organizacji może ją dezaktywować.");
        }

        [Fact]
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

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new DeactivateOrganizationCommand
            {
                Id = organization.Id,
                UserId = nonMember.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Tylko właściciel organizacji może ją dezaktywować.");
        }
    }
}