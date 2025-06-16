using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Organizations.Commands;

/// <summary>
/// Tests for UpdateOrganizationCommandHandler.
/// </summary>
public class UpdateOrganizationCommandHandlerTests : BaseUnitTest
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<UpdateOrganizationCommandHandler> _logger;
    private readonly UpdateOrganizationCommandHandler _handler;

    public UpdateOrganizationCommandHandlerTests()
    {
        _organizationRepository = RepositoryMockFactory.CreateSuccessfulOrganizationRepository();
        _logger = MockFactory.CreateLogger<UpdateOrganizationCommandHandler>();
        _handler = new UpdateOrganizationCommandHandler(_organizationRepository, _logger);
    }

    public class Handle : UpdateOrganizationCommandHandlerTests
    {
        [Fact]
        public async Task Should_UpdateOrganization_When_UserIsOwnerAndDataIsValid()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Organization Name",
                Description = "Updated Description",
                UserId = organization.OwnerId // User is the owner
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null); // Name is available

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Verify organization was updated
            organization.Name.Should().Be("Updated Organization Name");
            organization.Description.Should().Be("Updated Description");

            // Verify repository interactions
            await _organizationRepository.Received(1).GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).GetByNameAsync(command.Name, Arg.Any<CancellationToken>());
            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_UpdateOrganization_When_UserIsAdminMember()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var adminUserId = GenerateId();

            // Add admin role and member
            var adminRole = organization.AddRole("Admin", "Admin role");
            var adminMember = organization.AddMember(adminUserId, adminRole.Id);
            adminMember.SyncRoles(organization.Roles); // Ensure navigation property is synced

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated by Admin",
                Description = "Updated by admin member",
                UserId = adminUserId // User is admin member
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            organization.Name.Should().Be("Updated by Admin");
            organization.Description.Should().Be("Updated by admin member");

            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnForbidden_When_UserIsNotOwnerOrAdmin()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var regularUserId = GenerateId();

            // Add regular member (not admin)
            var memberRole = organization.AddRole("Member", "Regular member role");
            var member = organization.AddMember(regularUserId, memberRole.Id);
            member.SyncRoles(organization.Roles);

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Unauthorized Update",
                Description = "Should not be allowed",
                UserId = regularUserId // User is regular member
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Forbidden);
            result.Errors.Should().Contain(e => e.Contains("Brak uprawnień do aktualizacji organizacji"));

            // Verify no update was attempted
            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnForbidden_When_UserIsNotMemberAtAll()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var outsiderUserId = GenerateId(); // User not in organization

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Unauthorized Update",
                Description = "Should not be allowed",
                UserId = outsiderUserId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Forbidden);
            result.Errors.Should().Contain(e => e.Contains("Brak uprawnień do aktualizacji organizacji"));

            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_OrganizationDoesNotExist()
        {
            // Arrange
            var organizationId = GenerateId();
            var userId = GenerateId();
            var command = new UpdateOrganizationCommand
            {
                Id = organizationId,
                Name = "Non-existent Organization",
                Description = "Should not work",
                UserId = userId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono organizacji o ID {organizationId}"));

            // Verify no update was attempted
            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_OrganizationNameAlreadyExists()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var existingOrganization = OrganizationMother.ValidOrganization(); // Different organization with same name

            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Existing Name",
                Description = "Updated Description",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns(existingOrganization); // Name is taken by another organization

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Organizacja o podanej nazwie już istnieje"));

            // Verify no update was attempted
            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_AllowSameName_When_UpdatingWithCurrentName()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = organization.Name, // Same name as current
                Description = "Updated Description Only",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // GetByNameAsync should not be called when name doesn't change
            // But if it is called, it should return the same organization

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            organization.Description.Should().Be("Updated Description Only");

            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_GetByIdThrowsException()
        {
            // Arrange
            var organizationId = GenerateId();
            var userId = GenerateId();
            var command = new UpdateOrganizationCommand
            {
                Id = organizationId,
                Name = "Test Organization",
                Description = "Test Description",
                UserId = userId
            };

            var exception = new InvalidOperationException("Database connection failed");
            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Organization?>(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas aktualizacji organizacji"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));

            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_UpdateAsyncThrowsException()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Name",
                Description = "Updated Description",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            var exception = new InvalidOperationException("Update failed");
            _organizationRepository.UpdateAsync(organization, Arg.Any<CancellationToken>())
                .Returns(Task.FromException(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas aktualizacji organizacji"));
            result.Errors.Should().Contain(e => e.Contains("Update failed"));
        }

        [Fact]
        public async Task Should_LogSuccess_When_UpdateCompletes()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Name",
                Description = "Updated Description",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify logging occurred
            _logger.Received().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task Should_UseCorrectCancellationToken_When_Provided()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Name",
                Description = "Updated Description",
                UserId = organization.OwnerId
            };
            var cancellationToken = new CancellationToken();

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, cancellationToken)
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, cancellationToken)
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify the exact cancellation token was passed to all operations
            await _organizationRepository.Received(1).GetByIdWithMembersAndRolesAsync(command.Id, cancellationToken);
            await _organizationRepository.Received(1).GetByNameAsync(command.Name, cancellationToken);
            await _organizationRepository.Received(1).UpdateAsync(organization, cancellationToken);
        }

        [Theory]
        [InlineData("A", "B")]
        [InlineData("Valid Name", "Valid Description")]
        [InlineData("Very Long Organization Name That Should Still Work", "Very Long Description That Should Still Work")]
        public async Task Should_UpdateOrganization_When_NamesHaveVariousFormats(string name, string description)
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = name,
                Description = description,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            organization.Name.Should().Be(name);
            organization.Description.Should().Be(description);

            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_OrganizationNameIsEmpty()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "", // Empty string should fail
                Description = "Valid Description",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas aktualizacji organizacji"));

            // Verify no update was attempted
            await _organizationRepository.DidNotReceive().UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_UpdateOrganization_When_OrganizationNameIsWhitespace()
        {
            // Arrange - Guard.Against.NullOrEmpty allows whitespace
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "   ", // Whitespace should work
                Description = "Valid Description",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            organization.Name.Should().Be("   ");
            organization.Description.Should().Be("Valid Description");

            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_HandleInactiveOrganization_When_OrganizationIsDeactivated()
        {
            // Arrange
            var organization = OrganizationMother.InactiveOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "Updated Inactive Org",
                Description = "Updated Description",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Handler should allow updating inactive organizations
            organization.Name.Should().Be("Updated Inactive Org");
            organization.Description.Should().Be("Updated Description");
            organization.IsActive.Should().BeFalse(); // Should remain inactive

            await _organizationRepository.Received(1).UpdateAsync(organization, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_CallCorrectRepositoryMethods_When_HandlingCommand()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var command = new UpdateOrganizationCommand
            {
                Id = organization.Id,
                Name = "New Name",
                Description = "New Description",
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(organization);

            _organizationRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify that GetByIdWithMembersAndRolesAsync was called (not just GetByIdAsync)
            await _organizationRepository.Received(1).GetByIdWithMembersAndRolesAsync(command.Id, Arg.Any<CancellationToken>());
            await _organizationRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }
    }
}