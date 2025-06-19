using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for UpdateOrganizationCommandHandler.
/// Tests the complete flow from command to database update of organization.
/// </summary>
public class UpdateOrganizationCommandHandlerTests : BaseIntegrationTest
{
    public UpdateOrganizationCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : UpdateOrganizationCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_UpdateOrganization_When_ValidDataProvided()
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
                "Original Name",
                owner.Id,
                "Original description",
                "original-logo.jpg",
                "original-banner.jpg");

            organization.InitializeRoles();
            organization.InitializeOwner();

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Name",
                Description = "Updated description",
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization was updated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.Name.Should().Be("Updated Name");
            updatedOrganization.Description.Should().Be("Updated description");
            // LogoUrl and BannerUrl should remain unchanged
            updatedOrganization.LogoUrl.Should().Be("original-logo.jpg");
            updatedOrganization.BannerUrl.Should().Be("original-banner.jpg");
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

            var command = new UpdateOrganizationCommand
            {
                Id = nonExistentOrganizationId,
                Name = "New Name",
                Description = "New description",
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

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Name",
                Description = "Updated description",
                UserId = nonOwner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("Brak uprawnień"));
        }

        [Fact]
        public async Task Should_ReturnError_When_NameAlreadyExists()
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

            var organization1 = Organization.Create(
                "Organization 1",
                owner.Id,
                "Description 1",
                null,
                null);

            var organization2 = Organization.Create(
                "Organization 2",
                owner.Id,
                "Description 2",
                null,
                null);

            organization1.InitializeRoles();
            organization1.InitializeOwner();
            organization2.InitializeRoles();
            organization2.InitializeOwner();

            await DbContext.Organizations.AddRangeAsync(organization1, organization2);
            await SaveAndClearAsync();

            var command = new UpdateOrganizationCommand
            {
                Id = organization2.Id,
                Name = "Organization 1", // Same name as organization1
                Description = "Updated description",
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("już istnieje") || e.Contains("nazwa"));
        }

        [Fact]
        public async Task Should_UpdateOnlyProvidedFields_When_PartialDataProvided()
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
                "Original Name",
                owner.Id,
                "Original description",
                "original-logo.jpg",
                "original-banner.jpg");

            organization.InitializeRoles();
            organization.InitializeOwner();

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Name",
                Description = "Updated description",
                // LogoUrl and BannerUrl not provided - should remain unchanged
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization was updated correctly
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.Name.Should().Be("Updated Name");
            updatedOrganization.Description.Should().Be("Updated description");
            updatedOrganization.LogoUrl.Should().Be("original-logo.jpg"); // Should remain unchanged
            updatedOrganization.BannerUrl.Should().Be("original-banner.jpg"); // Should remain unchanged
        }

        [Fact]
        public async Task Should_ReturnError_When_NameIsEmpty()
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

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "", // Empty name
                Description = "Updated description",
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("nazwa") || e.Contains("wymagana"));
        }

        [Fact]
        public async Task Should_AllowUpdatingSameName_When_OrganizationKeepsItsName()
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
                "Original description",
                null,
                null);

            organization.InitializeRoles();
            organization.InitializeOwner();

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Test Organization", // Same name
                Description = "Updated description",
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify organization was updated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.Name.Should().Be("Test Organization");
            updatedOrganization.Description.Should().Be("Updated description");
        }
    }
}