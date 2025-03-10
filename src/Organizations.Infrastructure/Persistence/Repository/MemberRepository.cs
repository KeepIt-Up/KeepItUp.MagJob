namespace Organizations.Infrastructure.Persistence.Repository;

internal class MemberRepository(
    ApplicationDbContext context
    ) : BaseRepository<Member>(context), IMemberRepository
{
}