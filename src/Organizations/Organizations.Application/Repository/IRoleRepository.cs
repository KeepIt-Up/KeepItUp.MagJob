namespace Organizations.Application.Repository;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetWithPermissions(Guid id, CancellationToken cancellationToken);
    Task<PaginatedList<Role, T>> GetRolesByOrganizationId<T>(Guid organizationId, PaginationOptions paginationOptions);
}
