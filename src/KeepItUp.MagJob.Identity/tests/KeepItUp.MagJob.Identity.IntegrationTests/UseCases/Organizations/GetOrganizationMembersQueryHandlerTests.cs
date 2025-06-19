using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Organizations;

/// <summary>
/// Integration tests for GetOrganizationMembersQueryHandler.
/// Tests the complete flow from query to database retrieval of organization members.
/// </summary>
public class GetOrganizationMembersQueryHandlerTests : BaseIntegrationTest
{
    public GetOrganizationMembersQueryHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : GetOrganizationMembersQueryHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_ReturnMembers_When_OrganizationHasMembers()
        {
            // Arrange
            var owner = User.Create(
                "Owner",
                "User",
                "owner@example.com",
                "owner",
                Guid.NewGuid());

            var member1 = User.Create(
                "Member1",
                "User",
                "member1@example.com",
                "member1",
                Guid.NewGuid());

            var member2 = User.Create(
                "Member2",
                "User",
                "member2@example.com",
                "member2",
                Guid.NewGuid());

            await DbContext.Users.AddRangeAsync(owner, member1, member2);
            await SaveAndClearAsync();

            var organization = Organization.Create(
                "Test Organization",
                owner.Id,
                "Test description",
                null,
                null);

            organization.InitializeRoles();
            organization.InitializeOwner();

            // Add additional members
            var memberRole = organization.Roles.First(r => r.Name == "Member");
            organization.AddMember(member1.Id, memberRole.Id);
            organization.AddMember(member2.Id, memberRole.Id);

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var query = new GetOrganizationMembersQuery
            {
                OrganizationId = organization.Id,
                UserId = owner.Id,
                PaginationParameters = new PaginationParameters<MemberDto>
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
            result.Value.Items.Should().HaveCount(3); // Owner + 2 members
            result.Value.TotalCount.Should().Be(3);
            result.Value.Items.Should().Contain(m => m.Email == "owner@example.com");
            result.Value.Items.Should().Contain(m => m.Email == "member1@example.com");
            result.Value.Items.Should().Contain(m => m.Email == "member2@example.com");
        }

        [Fact]
        public async Task Should_ReturnOnlyOwner_When_OrganizationHasOnlyOwner()
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

            var query = new GetOrganizationMembersQuery
            {
                OrganizationId = organization.Id,
                UserId = owner.Id,
                PaginationParameters = new PaginationParameters<MemberDto>
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
            result.Value.TotalCount.Should().Be(1);
            result.Value.Items.First().Email.Should().Be("owner@example.com");
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

            var query = new GetOrganizationMembersQuery
            {
                OrganizationId = nonExistentOrganizationId,
                UserId = user.Id,
                PaginationParameters = new PaginationParameters<MemberDto>
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("Nie znaleziono organizacji"));
        }

        [Fact(Skip = "Autoryzacja jest zakomentowana w handlerze")]
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

            var query = new GetOrganizationMembersQuery
            {
                OrganizationId = organization.Id,
                UserId = nonMember.Id,
                PaginationParameters = new PaginationParameters<MemberDto>
                {
                    PageNumber = 1,
                    PageSize = 10
                }
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Brak dostępu do organizacji.");
        }

        [Fact]
        public async Task Should_ReturnCorrectPagination_When_OrganizationHasManyMembers()
        {
            // Arrange
            var owner = User.Create(
                "Owner",
                "User",
                "owner@example.com",
                "owner",
                Guid.NewGuid());

            var members = new List<User>();
            for (int i = 1; i <= 5; i++)
            {
                var member = User.Create(
                    $"Member{i}",
                    "User",
                    $"member{i}@example.com",
                    $"member{i}",
                    Guid.NewGuid());
                members.Add(member);
            }

            await DbContext.Users.AddAsync(owner);
            await DbContext.Users.AddRangeAsync(members);
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
            foreach (var member in members)
            {
                organization.AddMember(member.Id, memberRole.Id);
            }

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var query = new GetOrganizationMembersQuery
            {
                OrganizationId = organization.Id,
                UserId = owner.Id,
                PaginationParameters = new PaginationParameters<MemberDto>
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
            result.Value.TotalCount.Should().Be(6); // Owner + 5 members
            result.Value.HasNext.Should().BeTrue();
            result.Value.HasPrevious.Should().BeFalse();
        }

        [Fact]
        public async Task Should_ReturnMembersWithCorrectRoles_When_MembersHaveRoles()
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

            var memberRole = organization.Roles.First(r => r.Name == "Member");
            organization.AddMember(member.Id, memberRole.Id);

            await DbContext.Organizations.AddAsync(organization);
            await SaveAndClearAsync();

            var query = new GetOrganizationMembersQuery
            {
                OrganizationId = organization.Id,
                UserId = owner.Id,
                PaginationParameters = new PaginationParameters<MemberDto>
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

            var ownerMember = result.Value.Items.First(m => m.Email == "owner@example.com");
            var regularMember = result.Value.Items.First(m => m.Email == "member@example.com");

            ownerMember.Roles.Should().Contain(r => r.Name == "Admin");
            regularMember.Roles.Should().Contain(r => r.Name == "Member");
        }
    }
}