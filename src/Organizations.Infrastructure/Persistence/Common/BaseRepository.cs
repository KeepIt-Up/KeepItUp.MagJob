using Organizations.Domain.Common;

namespace Organizations.Infrastructure.Persistence.Common;

public class BaseRepository<T>(
    ApplicationDbContext context
    ) : IBaseRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context = context;

    public void Create(T entity)
    {
        Context.Add(entity);
    }

    public void Update(T entity)
    {
        Context.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        entity.DateDeleted = DateTimeOffset.UtcNow;
        Context.Update(entity);
    }

    public Task<T?> Get(Guid id, CancellationToken cancellationToken)
    {
        return Context.Set<T>().Where(x => !x.IsDeleted).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<T> GetAll()
    {
        return Context.Set<T>().Where(x => !x.IsDeleted);
    }
}
