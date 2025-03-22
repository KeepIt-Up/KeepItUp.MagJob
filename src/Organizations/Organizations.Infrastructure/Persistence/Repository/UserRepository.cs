namespace Organizations.Infrastructure.Persistence.Repository;

internal class UserRepository(
    ApplicationDbContext context
    ) : BaseRepository<User>(context), IUserRepository
{

    public async Task<PaginatedList<Invitation, T>> GetInvitationsByUserIdAsync<T>(Guid userId, PaginationOptions paginationOptions, CancellationToken cancellationToken)
    {
        return await PaginatedList<Invitation, T>.CreateAsync(Context.Set<Invitation>().Where(i => i.UserId == userId), paginationOptions);
    }
}
