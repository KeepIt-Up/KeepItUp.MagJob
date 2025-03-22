namespace Organizations.Infrastructure.Persistence.Repository;

internal class PermissionRepository(
    ApplicationDbContext context
    ) : BaseRepository<Permission>(context), IPermissionRepository
{
}