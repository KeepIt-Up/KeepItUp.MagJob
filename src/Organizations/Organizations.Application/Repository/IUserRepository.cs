namespace Organizations.Application.Repository;

public interface IUserRepository
{
    Task<PaginatedList<Invitation, T>> GetInvitationsByUserIdAsync<T>(Guid userId, PaginationOptions paginationOptions, CancellationToken cancellationToken);
}
