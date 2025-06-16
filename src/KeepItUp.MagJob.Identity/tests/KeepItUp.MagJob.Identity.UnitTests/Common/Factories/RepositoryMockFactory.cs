using NSubstitute;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.UnitTests.Core.UserAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.UnitTests.Core.InvitationAggregate;

namespace KeepItUp.MagJob.Identity.UnitTests.Common.Factories;

/// <summary>
/// Factory for creating mock repository objects with pre-configured behaviors.
/// Provides realistic scenarios for testing.
/// </summary>
public static class RepositoryMockFactory
{
    /// <summary>
    /// Creates a mock IUserRepository with successful operations.
    /// </summary>
    /// <returns>Mock user repository</returns>
    public static IUserRepository CreateSuccessfulUserRepository()
    {
        var repository = Substitute.For<IUserRepository>();

        // Setup successful operations
        repository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        repository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        repository.DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        return repository;
    }

    /// <summary>
    /// Creates a mock IUserRepository with pre-seeded users.
    /// </summary>
    /// <param name="users">Users to include in the repository</param>
    /// <returns>Mock user repository with seeded data</returns>
    public static IUserRepository CreateUserRepositoryWithUsers(params User[] users)
    {
        var repository = CreateSuccessfulUserRepository();

        // Setup retrieval methods
        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                return users.FirstOrDefault(u => u.Id == id);
            });

        repository.GetByExternalIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var externalId = callInfo.Arg<Guid>();
                return users.FirstOrDefault(u => u.ExternalId == externalId);
            });

        repository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var email = callInfo.Arg<string>();
                return users.FirstOrDefault(u => u.Email == email);
            });

        repository.GetActiveUsersAsync(Arg.Any<CancellationToken>())
            .Returns(users.Where(u => u.IsActive).ToList());

        repository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var ids = callInfo.Arg<IEnumerable<Guid>>();
                return users.Where(u => ids.Contains(u.Id)).ToList();
            });

        return repository;
    }

    /// <summary>
    /// Creates a mock IUserRepository that throws exceptions.
    /// </summary>
    /// <param name="exception">Exception to throw</param>
    /// <returns>Mock user repository that fails</returns>
    public static IUserRepository CreateFailingUserRepository(Exception exception)
    {
        var repository = Substitute.For<IUserRepository>();

        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<User?>(exception));

        repository.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<User>(exception));

        repository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(exception));

        return repository;
    }

    /// <summary>
    /// Creates a mock IOrganizationRepository with successful operations.
    /// </summary>
    /// <returns>Mock organization repository</returns>
    public static IOrganizationRepository CreateSuccessfulOrganizationRepository()
    {
        var repository = Substitute.For<IOrganizationRepository>();

        repository.AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Organization>());

        repository.UpdateAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        repository.DeleteAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        return repository;
    }

    /// <summary>
    /// Creates a mock IOrganizationRepository with pre-seeded organizations.
    /// </summary>
    /// <param name="organizations">Organizations to include in the repository</param>
    /// <returns>Mock organization repository with seeded data</returns>
    public static IOrganizationRepository CreateOrganizationRepositoryWithData(params Organization[] organizations)
    {
        var repository = CreateSuccessfulOrganizationRepository();

        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                return organizations.FirstOrDefault(o => o.Id == id);
            });

        return repository;
    }

    /// <summary>
    /// Creates a mock IInvitationRepository with successful operations.
    /// </summary>
    /// <returns>Mock invitation repository</returns>
    public static IInvitationRepository CreateSuccessfulInvitationRepository()
    {
        var repository = Substitute.For<IInvitationRepository>();

        repository.AddAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Invitation>());

        repository.UpdateAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        repository.DeleteAsync(Arg.Any<Invitation>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        return repository;
    }

    /// <summary>
    /// Creates a mock IInvitationRepository with pre-seeded invitations.
    /// </summary>
    /// <param name="invitations">Invitations to include in the repository</param>
    /// <returns>Mock invitation repository with seeded data</returns>
    public static IInvitationRepository CreateInvitationRepositoryWithData(params Invitation[] invitations)
    {
        var repository = CreateSuccessfulInvitationRepository();

        repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var id = callInfo.Arg<Guid>();
                return invitations.FirstOrDefault(i => i.Id == id);
            });

        return repository;
    }

    /// <summary>
    /// Creates a complete set of mock repositories for testing use cases.
    /// </summary>
    /// <returns>Tuple with all repository mocks</returns>
    public static (IUserRepository UserRepo, IOrganizationRepository OrgRepo, IInvitationRepository InvitationRepo)
        CreateAllRepositories()
    {
        return (
            CreateSuccessfulUserRepository(),
            CreateSuccessfulOrganizationRepository(),
            CreateSuccessfulInvitationRepository()
        );
    }

    /// <summary>
    /// Creates repositories with realistic test data.
    /// </summary>
    /// <returns>Tuple with repositories containing test data</returns>
    public static (IUserRepository UserRepo, IOrganizationRepository OrgRepo, IInvitationRepository InvitationRepo)
        CreateRepositoriesWithTestData()
    {
        var users = new[]
        {
            UserMother.ValidUser(),
            UserMother.AdminUser(),
            UserMother.InactiveUser()
        };

        var organizations = new[]
        {
            OrganizationMother.ValidOrganization(),
            OrganizationMother.InactiveOrganization()
        };

        var invitations = new[]
        {
            InvitationMother.ValidInvitation(),
            InvitationMother.ExpiredInvitation(),
            InvitationMother.AcceptedInvitation()
        };

        return (
            CreateUserRepositoryWithUsers(users),
            CreateOrganizationRepositoryWithData(organizations),
            CreateInvitationRepositoryWithData(invitations)
        );
    }
}