using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for UpdateOrganizationLogoCommandHandler.
/// Tests the complete flow from command to database update of organization logo.
/// </summary>
public class UpdateOrganizationLogoCommandHandlerTests : BaseIntegrationTest
{
    public UpdateOrganizationLogoCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : UpdateOrganizationLogoCommandHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_UpdateLogo_When_ValidImageProvidedByOwner()
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

            var logoFile = CreateTestImageFile("new-logo.jpg", "image/jpeg");

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = logoFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("new-logo");

            // Verify organization logo was updated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.LogoUrl.Should().NotBeNullOrEmpty();
            updatedOrganization.LogoUrl.Should().Contain("new-logo");
        }

        [Fact]
        public async Task Should_UpdateLogo_When_ValidImageProvidedByAdminMember()
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

            var logoFile = CreateTestImageFile("admin-logo.jpg", "image/jpeg");

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = logoFile,
                UserId = admin.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("admin-logo");
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
            var logoFile = CreateTestImageFile("logo.jpg", "image/jpeg");

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = nonExistentOrganizationId,
                LogoFile = logoFile,
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

            var logoFile = CreateTestImageFile("logo.jpg", "image/jpeg");

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = logoFile,
                UserId = outsider.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("permission") || e.Contains("member"));
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

            var logoFile = CreateTestImageFile("logo.jpg", "image/jpeg");

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = logoFile,
                UserId = member.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("permission") || e.Contains("member"));
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

            var invalidFile = CreateTestImageFile("document.txt", "text/plain");

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = invalidFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("files are allowed"));
        }

        [Fact]
        public async Task Should_ReturnError_When_FileTooLarge()
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

            var largeFile = CreateTestImageFile("large-logo.jpg", "image/jpeg", 10 * 1024 * 1024); // 10MB

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = largeFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("large") || e.Contains("size"));
        }

        [Fact]
        public async Task Should_ReplaceExistingLogo_When_OrganizationAlreadyHasOne()
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
                "old-logo.jpg",
                null);

            organization.InitializeRoles();
            organization.InitializeOwner();

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var newLogoFile = CreateTestImageFile("new-logo.jpg", "image/jpeg");

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = newLogoFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            result.Value.Should().Contain("new-logo");

            // Verify organization logo was updated
            var updatedOrganization = await DbContext.Organizations.FindAsync(organization.Id);
            updatedOrganization.Should().NotBeNull();
            updatedOrganization!.LogoUrl.Should().NotBeNullOrEmpty();
            updatedOrganization.LogoUrl.Should().Contain("new-logo");
            updatedOrganization.LogoUrl.Should().NotContain("old-logo");
        }

        [Fact]
        public async Task Should_ReturnError_When_FileIsEmpty()
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

            var emptyFile = CreateTestImageFile("empty.jpg", "image/jpeg", 0);

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = organization.Id,
                LogoFile = emptyFile,
                UserId = owner.Id
            };

            // Act
            var result = await Mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("required"));
        }

        private static IFormFile CreateTestImageFile(string fileName, string contentType, int sizeInBytes = 1024)
        {
            var content = new byte[sizeInBytes];
            for (int i = 0; i < sizeInBytes; i++)
            {
                content[i] = (byte)(i % 256);
            }

            var stream = new MemoryStream(content);
            var formFile = new FormFile(stream, 0, sizeInBytes, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }
    }
}