using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserOrganizations;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Users.Queries;

/// <summary>
/// Integration tests for GetUserOrganizationsQueryHandler.
/// Tests the complete flow from query to database retrieval of user organizations.
/// </summary>
public class GetUserOrganizationsQueryHandlerTests : BaseIntegrationTest
{
    public GetUserOrganizationsQueryHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : GetUserOrganizationsQueryHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_ReturnOrganizations_When_UserHasOrganizations()
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

            var organization1 = Organization.Create(
                "Organization 1",
                user.Id,
                "First organization",
                "logo1.jpg",
                "banner1.jpg");

            var organization2 = Organization.Create(
                "Organization 2",
                user.Id,
                "Second organization",
                "logo2.jpg",
                "banner2.jpg");

            organization1.InitializeRoles();
            organization1.InitializeOwner();
            organization2.InitializeRoles();
            organization2.InitializeOwner();

            await DbContext.Organizations.AddRangeAsync(organization1, organization2);
            await SaveAndClearAsync();

            var query = new GetUserOrganizationsQuery
            {
                UserId = user.Id,
                PaginationParameters = new PaginationParameters<OrganizationDto>
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(2);
            result.Value.TotalCount.Should().Be(2);
            result.Value.Items.Should().Contain(o => o.Name == "Organization 1");
            result.Value.Items.Should().Contain(o => o.Name == "Organization 2");
        }

        [Fact]
        public async Task Should_ReturnEmptyList_When_UserHasNoOrganizations()
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

            var query = new GetUserOrganizationsQuery
            {
                UserId = user.Id,
                PaginationParameters = new PaginationParameters<OrganizationDto>
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();

            var query = new GetUserOrganizationsQuery
            {
                UserId = nonExistentUserId,
                PaginationParameters = new PaginationParameters<OrganizationDto>
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("Nie znaleziono użytkownika"));
        }

        [Fact]
        public async Task Should_ReturnCorrectPagination_When_UserHasManyOrganizations()
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

            // Create 5 organizations
            var organizations = new List<Organization>();
            for (int i = 1; i <= 5; i++)
            {
                var org = Organization.Create(
                    $"Organization {i}",
                    user.Id,
                    $"Description {i}",
                    null,
                    null);
                org.InitializeRoles();
                org.InitializeOwner();
                organizations.Add(org);
            }

            await DbContext.Organizations.AddRangeAsync(organizations);
            await SaveAndClearAsync();

            var query = new GetUserOrganizationsQuery
            {
                UserId = user.Id,
                PaginationParameters = new PaginationParameters<OrganizationDto>
                {
                    PageNumber = 1,
                    PageSize = 3
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(3);
            result.Value.TotalCount.Should().Be(5);
            result.Value.HasNext.Should().BeTrue();
            result.Value.HasPrevious.Should().BeFalse();
        }

        [Fact]
        public async Task Should_ReturnOnlyUserOrganizations_When_MultipleUsersExist()
        {
            // Arrange
            var user1 = User.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "johndoe",
                Guid.NewGuid());

            var user2 = User.Create(
                "Jane",
                "Smith",
                "jane.smith@example.com",
                "janesmith",
                Guid.NewGuid());

            await DbContext.Users.AddRangeAsync(user1, user2);
            await SaveAndClearAsync();

            var org1 = Organization.Create(
                "User1 Organization",
                user1.Id,
                "Description 1",
                null,
                null);

            var org2 = Organization.Create(
                "User2 Organization",
                user2.Id,
                "Description 2",
                null,
                null);

            org1.InitializeRoles();
            org1.InitializeOwner();
            org2.InitializeRoles();
            org2.InitializeOwner();

            await DbContext.Organizations.AddRangeAsync(org1, org2);
            await SaveAndClearAsync();

            var query = new GetUserOrganizationsQuery
            {
                UserId = user1.Id,
                PaginationParameters = new PaginationParameters<OrganizationDto>
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Items.First().Name.Should().Be("User1 Organization");
            result.Value.TotalCount.Should().Be(1);
        }

        [Fact]
        public async Task Should_ReturnOrganizationsWithCorrectData_When_UserHasOrganizations()
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
                "test-logo.jpg",
                "test-banner.jpg");

            organization.InitializeRoles();
            organization.InitializeOwner();

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var query = new GetUserOrganizationsQuery
            {
                UserId = user.Id,
                PaginationParameters = new PaginationParameters<OrganizationDto>
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(1);

            var orgDto = result.Value.Items.First();
            orgDto.Id.Should().Be(organization.Id);
            orgDto.Name.Should().Be("Test Organization");
            orgDto.Description.Should().Be("Test description");
            orgDto.LogoUrl.Should().Be("test-logo.jpg");
            orgDto.BannerUrl.Should().Be("test-banner.jpg");
            orgDto.IsActive.Should().BeTrue();
        }
    }
}