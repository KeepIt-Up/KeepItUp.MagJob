using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.InvitationAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Common.Seeding;

/// <summary>
/// Provides utilities for seeding test databases with realistic data.
/// Uses only existing Mother class methods for reliability.
/// </summary>
public static class TestDataSeeder
{
    /// <summary>
    /// Seeds a basic dataset for simple tests.
    /// </summary>
    public static TestDataSet SeedBasicDataSet()
    {
        return new TestDataSet
        {
            Users = new List<User>
            {
                UserMother.ValidUser(),
                UserMother.AdminUser()
            },
            Organizations = new List<Organization>
            {
                OrganizationMother.ValidOrganization()
            },
            Invitations = new List<Invitation>
            {
                InvitationMother.ValidInvitation()
            }
        };
    }

    /// <summary>
    /// Seeds a comprehensive dataset for complex tests.
    /// </summary>
    public static TestDataSet SeedComprehensiveDataSet()
    {
        return new TestDataSet
        {
            Users = new List<User>
            {
                UserMother.ValidUser(),
                UserMother.AdminUser(),
                UserMother.InactiveUser(),
                UserMother.UserForEdgeCases(),
                UserMother.UserWithProfile()
            },
            Organizations = new List<Organization>
            {
                OrganizationMother.ValidOrganization(),
                OrganizationMother.CompleteOrganization(),
                OrganizationMother.InactiveOrganization(),
                OrganizationMother.OrganizationWithRoles(),
                OrganizationMother.MinimalOrganization()
            },
            Invitations = new List<Invitation>
            {
                InvitationMother.ValidInvitation(),
                InvitationMother.ExpiredInvitation(),
                InvitationMother.AcceptedInvitation(),
                InvitationMother.RejectedInvitation(),
                InvitationMother.InvitationExpiringSoon()
            }
        };
    }

    /// <summary>
    /// Creates multiple users using factory methods.
    /// </summary>
    public static List<User> CreateMultipleUsers(int count = 5)
    {
        return UserMother.MultipleUsers(count);
    }

    /// <summary>
    /// Creates multiple organizations using factory methods.
    /// </summary>
    public static List<Organization> CreateMultipleOrganizations(int count = 3)
    {
        return OrganizationMother.MultipleOrganizations(count);
    }

    /// <summary>
    /// Creates multiple invitations using factory methods.
    /// </summary>
    public static List<Invitation> CreateMultipleInvitations(int count = 5)
    {
        return InvitationMother.MultipleInvitations(count);
    }
}

/// <summary>
/// Contains seeded test data for all entities.
/// </summary>
public class TestDataSet
{
    public List<User> Users { get; set; } = new();
    public List<Organization> Organizations { get; set; } = new();
    public List<Invitation> Invitations { get; set; } = new();

    public int TotalCount => Users.Count + Organizations.Count + Invitations.Count;
    public string Summary => $"Users: {Users.Count}, Organizations: {Organizations.Count}, Invitations: {Invitations.Count}";
}