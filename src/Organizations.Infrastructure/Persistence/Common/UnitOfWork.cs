using Organizations.Infrastructure.Persistence;

namespace Organizations.Persistence.Repository;

public class UnitOfWork(
    ApplicationDbContext Context
    ) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }
}
