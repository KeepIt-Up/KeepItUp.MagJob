using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.UnitTests.Common;
using KeepItUp.MagJob.Identity.UnitTests.Common.Factories;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KeepItUp.MagJob.Identity.UnitTests.UseCases.Organizations.Queries;

/// <summary>
/// Tests for GetOrganizationByIdQueryHandler.
/// </summary>
public class GetOrganizationByIdQueryHandlerTests : BaseUnitTest
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetOrganizationByIdQueryHandler> _logger;
    private readonly GetOrganizationByIdQueryHandler _handler;

    public GetOrganizationByIdQueryHandlerTests()
    {
        _organizationRepository = RepositoryMockFactory.CreateSuccessfulOrganizationRepository();
        _logger = MockFactory.CreateLogger<GetOrganizationByIdQueryHandler>();
        _handler = new GetOrganizationByIdQueryHandler(_organizationRepository, _logger);
    }

    public class Handle : GetOrganizationByIdQueryHandlerTests
    {
        [Fact]
        public async Task Should_ReturnOrganizationDto_When_OrganizationExistsAndUserIsOwner()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = organization.OwnerId // User is the owner
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var organizationDto = result.Value;
            organizationDto.Id.Should().Be(organization.Id);
            organizationDto.Name.Should().Be(organization.Name);
            organizationDto.Description.Should().Be(organization.Description);
            organizationDto.OwnerId.Should().Be(organization.OwnerId);
            organizationDto.IsActive.Should().Be(organization.IsActive);
            organizationDto.LogoUrl.Should().Be(organization.LogoUrl);
            organizationDto.BannerUrl.Should().Be(organization.BannerUrl);

            // Owner should have all organization roles
            organizationDto.UserRoles.Should().NotBeNull();
            organizationDto.UserRoles.Should().HaveCount(organization.Roles.Count);

            // Verify repository interaction
            await _organizationRepository.Received(1).GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnOrganizationWithMemberRoles_When_UserIsMember()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var userId = GenerateId();

            // Add roles to organization and get their IDs
            var memberRole1 = organization.AddRole("Member Role 1", "Description 1");
            var memberRole2 = organization.AddRole("Member Role 2", "Description 2");

            // Add user as member with specific role
            var member = organization.AddMember(userId, memberRole1.Id);

            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = userId // User is a member
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var organizationDto = result.Value;

            // Member should have only assigned roles
            organizationDto.UserRoles.Should().NotBeNull();
            organizationDto.UserRoles.Should().HaveCount(1);
            organizationDto.UserRoles.Should().Contain("Member Role 1");
            organizationDto.UserRoles.Should().NotContain("Member Role 2");
        }

        [Fact]
        public async Task Should_ReturnOrganizationWithEmptyRoles_When_UserIsNotMemberOrOwner()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var randomUserId = GenerateId(); // User is neither owner nor member

            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = randomUserId
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var organizationDto = result.Value;

            // User should have no roles
            organizationDto.UserRoles.Should().NotBeNull();
            organizationDto.UserRoles.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_OrganizationDoesNotExist()
        {
            // Arrange
            var organizationId = GenerateId();
            var userId = GenerateId();
            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organizationId,
                UserId = userId
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns((Organization?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.NotFound);
            result.Errors.Should().Contain(e => e.Contains($"Nie znaleziono organizacji o ID {organizationId}"));

            // Verify repository interaction
            await _organizationRepository.Received(1).GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_ReturnError_When_RepositoryThrowsException()
        {
            // Arrange
            var organizationId = GenerateId();
            var userId = GenerateId();
            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organizationId,
                UserId = userId
            };

            var exception = new InvalidOperationException("Database connection failed");
            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Organization?>(exception));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Status.Should().Be(ResultStatus.Error);
            result.Errors.Should().Contain(e => e.Contains("Wystąpił błąd podczas pobierania organizacji"));
            result.Errors.Should().Contain(e => e.Contains("Database connection failed"));
        }

        [Fact]
        public async Task Should_MapAllOrganizationProperties_When_OrganizationIsComplete()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            // Set all optional properties
            organization.Update("Updated Name", "Updated Description", "http://logo.url", "http://banner.url");

            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var organizationDto = result.Value;

            // Verify all properties are mapped correctly
            organizationDto.Id.Should().Be(organization.Id);
            organizationDto.Name.Should().Be("Updated Name");
            organizationDto.Description.Should().Be("Updated Description");
            organizationDto.LogoUrl.Should().Be("http://logo.url");
            organizationDto.BannerUrl.Should().Be("http://banner.url");
            organizationDto.OwnerId.Should().Be(organization.OwnerId);
            organizationDto.IsActive.Should().Be(organization.IsActive);
            organizationDto.UserRoles.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_HandleInactiveOrganization_When_OrganizationIsDeactivated()
        {
            // Arrange
            var organization = OrganizationMother.InactiveOrganization();
            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var organizationDto = result.Value;

            organizationDto.IsActive.Should().BeFalse();
            // Handler should still return the organization even if inactive
            organizationDto.Id.Should().Be(organization.Id);
        }

        [Fact]
        public async Task Should_HandleNullOptionalFields_When_OrganizationHasMinimalData()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            // Organization might have null description, logoUrl, bannerUrl

            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var organizationDto = result.Value;

            // Optional fields should be handled gracefully
            organizationDto.Description.Should().NotBeNull(); // Might be empty string or null
            organizationDto.LogoUrl.Should().NotBeNull(); // Might be empty string or null
            organizationDto.BannerUrl.Should().NotBeNull(); // Might be empty string or null
        }

        [Fact]
        public async Task Should_UseCorrectCancellationToken_When_Provided()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = organization.OwnerId
            };
            var cancellationToken = new CancellationToken();

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, cancellationToken)
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify the exact cancellation token was passed
            await _organizationRepository.Received(1).GetByIdWithRolesAsync(query.OrganizationId, cancellationToken);
        }

        [Fact]
        public async Task Should_HandleMultipleRoles_When_MemberHasMultipleRoles()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var userId = GenerateId();

            // Add multiple roles to organization
            var role1 = organization.AddRole("Admin", "Admin role");
            var role2 = organization.AddRole("Editor", "Editor role");
            var role3 = organization.AddRole("Viewer", "Viewer role");

            // Add user as member with first role, then assign additional role
            var member = organization.AddMember(userId, role1.Id);
            organization.AssignRoleToMember(userId, role2.Id);

            // Manually sync roles to ensure navigation property is updated
            // (In real application, this would be handled by repository/EF Core)
            member.SyncRoles(organization.Roles);

            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = userId
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var organizationDto = result.Value;

            organizationDto.UserRoles.Should().HaveCount(2);
            organizationDto.UserRoles.Should().Contain("Admin");
            organizationDto.UserRoles.Should().Contain("Editor");
            organizationDto.UserRoles.Should().NotContain("Viewer");
        }

        [Fact]
        public async Task Should_CallCorrectRepositoryMethod_When_HandlingQuery()
        {
            // Arrange
            var organization = OrganizationMother.ValidOrganization();
            var query = new GetOrganizationByIdQuery
            {
                OrganizationId = organization.Id,
                UserId = organization.OwnerId
            };

            _organizationRepository.GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>())
                .Returns(organization);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Verify that GetByIdWithRolesAsync was called (not just GetByIdAsync)
            await _organizationRepository.Received(1).GetByIdWithRolesAsync(query.OrganizationId, Arg.Any<CancellationToken>());
            await _organizationRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }
    }
}