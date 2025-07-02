using FluentAssertions;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

public class CreateOrganizationCommandHandlerTests : BaseIntegrationTest
{
    public CreateOrganizationCommandHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task Should_CreateOrganization_When_ValidDataProvided()
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

        var command = new CreateOrganizationCommand
        {
            Name = "Test Organization",
            Description = "Test organization description",
            OwnerId = user.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var organization = await DbContext.Organizations
            .Include(o => o.Roles)
            .Include(o => o.Members)
            .FirstOrDefaultAsync(o => o.Id == result.Value);

        organization.Should().NotBeNull();
        organization!.Name.Should().Be(command.Name);
        organization.Description.Should().Be(command.Description);
        organization.OwnerId.Should().Be(user.Id);
        organization.IsActive.Should().BeTrue();
        organization.Roles.Should().NotBeEmpty();
        organization.Members.Should().ContainSingle()
            .Which.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Should_CreateOrganization_When_DescriptionIsNull()
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

        var command = new CreateOrganizationCommand
        {
            Name = "Test Organization",
            Description = null,
            OwnerId = user.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var organization = await DbContext.Organizations.FirstOrDefaultAsync(o => o.Id == result.Value);
        organization.Should().NotBeNull();
        organization!.Description.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnError_When_UserNotFound()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var command = new CreateOrganizationCommand
        {
            Name = "Test Organization",
            OwnerId = nonExistentUserId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain($"Nie znaleziono użytkownika o ID {nonExistentUserId}.");
    }

    [Fact]
    public async Task Should_ReturnError_When_OrganizationNameAlreadyExists()
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

        var existingOrganization = Organization.Create(
            "Existing Organization",
            user.Id,
            "Existing description",
            null,
            null);

        await DbContext.Organizations.AddAsync(existingOrganization);
        await SaveAndClearAsync();

        var command = new CreateOrganizationCommand
        {
            Name = existingOrganization.Name,
            OwnerId = user.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain($"Organizacja o nazwie '{existingOrganization.Name}' już istnieje.");
    }

    [Fact]
    public async Task Should_InitializeDefaultRoles_When_OrganizationCreated()
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

        var command = new CreateOrganizationCommand
        {
            Name = "Test Organization",
            OwnerId = user.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var organization = await DbContext.Organizations
            .Include(o => o.Roles)
            .FirstOrDefaultAsync(o => o.Id == result.Value);

        organization.Should().NotBeNull();
        organization!.Roles.Should().NotBeEmpty();
        organization.Roles.Should().Contain(r => r.Name == "Admin");
        organization.Roles.Should().Contain(r => r.Name == "Member");
        organization.Roles.Should().Contain(r => r.Name == "Guest");
    }

    [Fact]
    public async Task Should_AssignOwnerRole_When_OrganizationCreated()
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

        var command = new CreateOrganizationCommand
        {
            Name = "Test Organization",
            OwnerId = user.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var organization = await DbContext.Organizations
            .Include(o => o.Members)
            .Include(o => o.Roles)
            .FirstOrDefaultAsync(o => o.Id == result.Value);

        organization.Should().NotBeNull();
        organization!.Members.Should().ContainSingle();

        var member = organization.Members.First();
        member.UserId.Should().Be(user.Id);

        var adminRole = organization.Roles.First(r => r.Name == "Admin");
        member.HasRole(adminRole.Id).Should().BeTrue();
    }

    [Fact]
    public async Task Should_CreateOrganizationWithUniqueId_When_MultipleOrganizationsCreated()
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

        var command1 = new CreateOrganizationCommand
        {
            Name = "Organization 1",
            OwnerId = user.ExternalId
        };
        var command2 = new CreateOrganizationCommand
        {
            Name = "Organization 2",
            OwnerId = user.ExternalId
        };

        // Act
        var result1 = await Mediator.Send(command1);
        var result2 = await Mediator.Send(command2);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        result1.Value.Should().NotBe(result2.Value);

        var org1 = await DbContext.Organizations.FirstOrDefaultAsync(o => o.Id == result1.Value);
        var org2 = await DbContext.Organizations.FirstOrDefaultAsync(o => o.Id == result2.Value);

        org1.Should().NotBeNull();
        org2.Should().NotBeNull();
        org1!.Id.Should().NotBe(org2!.Id);
    }

    [Fact]
    public async Task Should_HandleException_When_RepositoryThrows()
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

        var command = new CreateOrganizationCommand
        {
            Name = new string('A', 1000), // Very long name that might cause issues
            OwnerId = user.ExternalId
        };

        // Act
        var result = await Mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Contains("Wystąpił błąd podczas tworzenia organizacji"));
    }
}