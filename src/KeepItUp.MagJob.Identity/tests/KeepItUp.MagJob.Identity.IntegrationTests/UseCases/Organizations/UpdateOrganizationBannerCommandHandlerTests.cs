using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for UpdateOrganizationBannerCommandHandler.
/// Tests the complete flow from command to database update of organization banner.
/// </summary>
public class UpdateOrganizationBannerCommandHandlerTests : BaseIntegrationTest
{
    public UpdateOrganizationBannerCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : UpdateOrganizationBannerCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_UpdateBanner_When_ValidImageProvidedByOwner()
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

            var bannerFile = CreateTestImageFile("new-banner.jpg", "image/jpeg");

            var command = new UpdateOrganizationBannerCommand
            {
                OrganizationId = organization.Id,
                BannerFile = bannerFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("new-banner");

            // Verify organization banner was updated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.BannerUrl.Should().NotBeNullOrEmpty();
            updatedOrganization.BannerUrl.Should().Contain("new-banner");
        }

        [Fact]
        public async Task Should_UpdateBanner_When_ValidImageProvidedByAdminMember()
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

            var bannerFile = CreateTestImageFile("admin-banner.jpg", "image/jpeg");

            var command = new UpdateOrganizationBannerCommand
            {
                OrganizationId = organization.Id,
                BannerFile = bannerFile,
                UserId = admin.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("admin-banner");
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
            var bannerFile = CreateTestImageFile("banner.jpg", "image/jpeg");

            var command = new UpdateOrganizationBannerCommand
            {
                OrganizationId = nonExistentOrganizationId,
                BannerFile = bannerFile,
                UserId = user.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("Organization") && e.Contains("not found"));
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

            var outsider = User.Create(
                "Outsider",
                "User",
                "outsider@example.com",
                "outsider",
                Guid.NewGuid());

            await DbContext.Users.AddRangeAsync(owner, outsider);
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

            var bannerFile = CreateTestImageFile("banner.jpg", "image/jpeg");

            var command = new UpdateOrganizationBannerCommand
            {
                OrganizationId = organization.Id,
                BannerFile = bannerFile,
                UserId = outsider.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Forbidden);
            result.Errors.Should().Contain(e => e.Contains("permission"));
        }

        [Fact]
        public async Task Should_ReturnError_When_InvalidFileTypeProvided()
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

            var bannerFile = CreateTestImageFile("invalid.txt", "text/plain");

            var command = new UpdateOrganizationBannerCommand
            {
                OrganizationId = organization.Id,
                BannerFile = bannerFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain("Only image/jpeg, image/jpg, image/png, image/gif, image/webp files are allowed for banner.");
        }

        [Fact]
        public async Task Should_ReplaceExistingBanner_When_OrganizationAlreadyHasOne()
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
                "existing-banner.jpg");

            organization.InitializeRoles();
            organization.InitializeOwner();

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var bannerFile = CreateTestImageFile("new-banner.jpg", "image/jpeg");

            var command = new UpdateOrganizationBannerCommand
            {
                OrganizationId = organization.Id,
                BannerFile = bannerFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("new-banner");
            result.Value.Should().NotContain("existing-banner");

            // Verify organization banner was updated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.BannerUrl.Should().NotBeNullOrEmpty();
            updatedOrganization.BannerUrl.Should().Contain("new-banner");
            updatedOrganization.BannerUrl.Should().NotContain("existing-banner");
        }

        private static IFormFile CreateTestImageFile(string fileName, string contentType, int sizeInBytes = 1024)
        {
            var stream = new MemoryStream(new byte[sizeInBytes]);
            return new FormFile(stream, 0, sizeInBytes, "bannerFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }
}