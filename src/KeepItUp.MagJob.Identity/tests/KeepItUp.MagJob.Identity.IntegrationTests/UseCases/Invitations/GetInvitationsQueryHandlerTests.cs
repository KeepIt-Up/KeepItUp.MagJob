using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.IntegrationTests.Infrastructure;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Queries.GetInvitations;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Queries;
using KeepItUp.MagJob.Identity.SharedKernel.Pagination;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.IntegrationTests.UseCases.Invitations;

/// <summary>
/// Integration tests for GetInvitationsQueryHandler.
/// Tests the complete flow from query to database retrieval of invitations.
/// </summary>
public class GetInvitationsQueryHandlerTests : BaseIntegrationTest
{
    public GetInvitationsQueryHandlerTests(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    public class Handle : GetInvitationsQueryHandlerTests
    {
        public Handle(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task Should_ReturnInvitations_When_OrganizationHasInvitations()
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

            var memberRole = organization.Roles.First(r => r.Name == "Member");

            var invitation1 = Invitation.Create(
                organization.Id,
                "invitee1@example.com",
                memberRole.Id);

            var invitation2 = Invitation.Create(
                organization.Id,
                "invitee2@example.com",
                memberRole.Id);

            await DbContext.Invitations.AddRangeAsync(invitation1, invitation2);
            await SaveAndClearAsync();

            var query = new GetInvitationsQuery
            {
                OrganizationId = organization.Id
            };
            query.PaginationParameters = new PaginationParameters<InvitationDto>
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(2);
            result.Value.TotalCount.Should().Be(2);
            result.Value.Items.Should().Contain(i => i.Email == "invitee1@example.com");
            result.Value.Items.Should().Contain(i => i.Email == "invitee2@example.com");
        }

        [Fact]
        public async Task Should_ReturnEmptyList_When_OrganizationHasNoInvitations()
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

            var query = new GetInvitationsQuery
            {
                OrganizationId = organization.Id
            };
            query.PaginationParameters = new PaginationParameters<InvitationDto>
            {
                PageNumber = 1,
                PageSize = 10
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

            var query = new GetInvitationsQuery
            {
                OrganizationId = nonExistentOrganizationId
            };
            query.PaginationParameters = new PaginationParameters<InvitationDto>
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("Nie znaleziono organizacji"));
        }

        [Fact]
        public async Task Should_ReturnResults_When_FilteringByEmail()
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

            var memberRole = organization.Roles.First(r => r.Name == "Member");

            // Create multiple invitations with different emails
            var invitation1 = Invitation.Create(
                organization.Id,
                "test1@example.com",
                memberRole.Id);

            var invitation2 = Invitation.Create(
                organization.Id,
                "test2@example.com",
                memberRole.Id);

            var invitation3 = Invitation.Create(
                organization.Id,
                "different@example.com",
                memberRole.Id);

            await DbContext.Invitations.AddRangeAsync(invitation1, invitation2, invitation3);
            await SaveAndClearAsync();

            var query = new GetInvitationsQuery
            {
                OrganizationId = organization.Id,
                Email = "test1@example.com"
            };
            query.PaginationParameters = new PaginationParameters<InvitationDto>
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Items.First().Email.Should().Be("test1@example.com");
        }

        [Fact]
        public async Task Should_ReturnCorrectPagination_When_OrganizationHasManyInvitations()
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

            var memberRole = organization.Roles.First(r => r.Name == "Member");

            // Create 5 invitations
            var invitations = new List<Invitation>();
            for (int i = 1; i <= 5; i++)
            {
                var invitation = Invitation.Create(
                    organization.Id,
                    $"invitee{i}@example.com",
                    memberRole.Id);
                invitations.Add(invitation);
            }

            await DbContext.Invitations.AddRangeAsync(invitations);
            await SaveAndClearAsync();

            var query = new GetInvitationsQuery
            {
                OrganizationId = organization.Id
            };
            query.PaginationParameters = new PaginationParameters<InvitationDto>
            {
                PageNumber = 1,
                PageSize = 3
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
        public async Task Should_ReturnInvitationsWithCorrectData_When_InvitationsExist()
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

            var memberRole = organization.Roles.First(r => r.Name == "Member");

            var invitation = Invitation.Create(
                organization.Id,
                "invitee@example.com",
                memberRole.Id);

            await DbContext.Invitations.AddAsync(invitation);
            await SaveAndClearAsync();

            var query = new GetInvitationsQuery
            {
                OrganizationId = organization.Id
            };
            query.PaginationParameters = new PaginationParameters<InvitationDto>
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await Mediator.Send(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(1);

            var invitationDto = result.Value.Items.First();
            invitationDto.Id.Should().Be(invitation.Id);
            invitationDto.OrganizationId.Should().Be(organization.Id);
            invitationDto.Email.Should().Be("invitee@example.com");
            invitationDto.Token.Should().NotBeNullOrEmpty();
            invitationDto.Status.Should().Be("Pending");
            invitationDto.IsExpired.Should().BeFalse();
            invitationDto.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            invitationDto.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }
    }
}