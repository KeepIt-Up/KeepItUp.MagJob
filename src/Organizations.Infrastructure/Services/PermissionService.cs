// using Organizations.API.Common.Services;
// using Organizations.API.Repositories;

// public interface IPermissionService
// {
//     Task<List<Permission>> GetPermissions(CancellationToken cancellationToken = default);
// }

// public class PermissionService : BaseService<Permission>, IPermissionService
// {
//     private readonly IPermissionRepository _repository;
//     public PermissionService(IPermissionRepository repository) : base(repository)
//     {
//         _repository = repository;
//     }

//     public async Task<List<Permission>> GetPermissions(CancellationToken cancellationToken = default)
//     {
//         return await _repository.GetAllAsync(cancellationToken);
//     }
// }