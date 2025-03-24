// using Microsoft.EntityFrameworkCore;
// using Organizations.API.Common.Models;
// using Organizations.API.Common.Services;
// using Organizations.API.Repositories;

// public interface IRoleService : IBaseService<Role>
// {
//     Task<List<RoleResponse>> GetRolesByOrganizationId(int organizationId, CancellationToken cancellationToken = default);
//     Task<PaginatedList<RoleResponse>> GetRolesByOrganizationIdPagination(int organizationId, PaginationOptions payload, CancellationToken cancellationToken = default);
//     Task<RoleResponse> CreateRole(string name, int organizationId, CancellationToken cancellationToken = default);
//     Task<RoleResponse> UpdateRole(int id, UpdateRolePayload payload, CancellationToken cancellationToken = default);
//     Task<RoleResponse> AddPermissionsToRole(int id, List<int> permissionIds, CancellationToken cancellationToken = default);
//     Task<RoleResponse> RemovePermissionsFromRole(int id, List<int> permissionIds, CancellationToken cancellationToken = default);
//     Task<RoleResponse> AddMembersToRole(int id, List<int> memberIds, CancellationToken cancellationToken = default);
//     Task<RoleResponse> RemoveMembersFromRole(int id, List<int> memberIds, CancellationToken cancellationToken = default);
// }

// public class RoleService : BaseService<Role>, IRoleService
// {
//     private readonly IRoleRepository _roleRepository;
//     private readonly IPermissionRepository _permissionRepository;
//     private readonly IMemberRepository _memberRepository;
//     public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IMemberRepository memberRepository) : base(roleRepository)
//     {
//         _roleRepository = roleRepository;
//         _permissionRepository = permissionRepository;
//         _memberRepository = memberRepository;
//     }

//     public async Task<List<RoleResponse>> GetRolesByOrganizationId(int organizationId, CancellationToken cancellationToken = default)
//     {
//         var roles = await _roleRepository.GetRolesByOrganizationId(organizationId, cancellationToken);
//         return roles.Select(r => new RoleResponse()
//         {
//             Id = r.Id,
//             Name = r.Name,
//             Description = r.Description,
//             Color = r.Color,
//             OrganizationId = r.OrganizationId,
//             Members = r.Members.Select(m => new MemberResponse() { Id = m.Id, FirstName = m.FirstName, LastName = m.LastName }).ToList(),
//             Permissions = r.Permissions.Select(p => new PermissionResponse() { Id = p.Id, Name = p.Name }).ToList()
//         }).ToList();
//     }

//     public async Task<PaginatedList<RoleResponse>> GetRolesByOrganizationIdPagination(int organizationId, PaginationOptions payload, CancellationToken cancellationToken = default)
//     {
//         var roles = _roleRepository.AsQueryable().Where(r => r.OrganizationId == organizationId).Select(r => new RoleResponse()
//         {
//             Id = r.Id,
//             Name = r.Name,
//             Description = r.Description,
//             Color = r.Color,
//             OrganizationId = r.OrganizationId,
//             Members = r.Members.Select(m => new MemberResponse() { Id = m.Id, FirstName = m.FirstName, LastName = m.LastName }).ToList(),
//             Permissions = r.Permissions.Select(p => new PermissionResponse() { Id = p.Id, Name = p.Name }).ToList()
//         });
//         return await PaginatedList<RoleResponse>.CreateAsync(roles.AsNoTracking(), payload);
//     }

//     public async Task<RoleResponse> CreateRole(string name, int organizationId, CancellationToken cancellationToken = default)
//     {
//         var role = Role.Create(name, organizationId, null, null);
//         var createdRole = await _roleRepository.CreateAsync(role, cancellationToken);
//         return new RoleResponse()
//         {
//             Id = createdRole.Id,
//             Name = createdRole.Name,
//             Description = createdRole.Description,
//         };
//     }

//     public async Task<RoleResponse> UpdateRole(int id, UpdateRolePayload payload, CancellationToken cancellationToken = default)
//     {
//         var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
//         if (role == null)
//         {
//             throw new KeyNotFoundException($"Role with id {id} not found");
//         }
//         role.Name = payload.Name;
//         role.Description = payload.Description;
//         role.Color = payload.Color;
//         var updatedRole = await _roleRepository.UpdateAsync(role, cancellationToken);
//         return new RoleResponse()
//         {
//             Id = updatedRole.Id,
//             Name = updatedRole.Name,
//             Color = updatedRole.Color,
//             Description = updatedRole.Description,
//         };
//     }

//     public async Task<RoleResponse> AddPermissionsToRole(int id, List<int> permissionIds, CancellationToken cancellationToken = default)
//     {
//         var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
//         if (role == null)
//         {
//             throw new KeyNotFoundException($"Role with id {id} not found");
//         }

//         var permissions = await _permissionRepository.AsQueryable().Where(p => permissionIds.Contains(p.Id)).ToListAsync(cancellationToken);
//         role.AddPermissions(permissions);
//         var updatedRole = await _roleRepository.UpdateAsync(role, cancellationToken);
//         return new RoleResponse()
//         {
//             Id = updatedRole.Id,
//             Name = updatedRole.Name,
//             Description = updatedRole.Description,
//         };
//     }

//     public async Task<RoleResponse> RemovePermissionsFromRole(int id, List<int> permissionIds, CancellationToken cancellationToken = default)
//     {
//         var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
//         if (role == null)
//         {
//             throw new KeyNotFoundException($"Role with id {id} not found");
//         }
//         var permissions = await _permissionRepository.AsQueryable().Where(p => permissionIds.Contains(p.Id)).ToListAsync(cancellationToken);
//         role.RemovePermissions(permissions);
//         var updatedRole = await _roleRepository.UpdateAsync(role, cancellationToken);
//         return new RoleResponse()
//         {
//             Id = updatedRole.Id,
//             Name = updatedRole.Name,
//             Description = updatedRole.Description,
//         };
//     }

//     public async Task<RoleResponse> AddMembersToRole(int id, List<int> memberIds, CancellationToken cancellationToken = default)
//     {
//         var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
//         if (role == null)
//         {
//             throw new KeyNotFoundException($"Role with id {id} not found");
//         }
//         var members = await _memberRepository.AsQueryable().Where(m => memberIds.Contains(m.Id)).ToListAsync(cancellationToken);
//         var membersToAdd = members.Where(m => memberIds.Contains(m.Id)).ToList();
//         role.AddMembers(membersToAdd);
//         var updatedRole = await _roleRepository.UpdateAsync(role, cancellationToken);
//         return new RoleResponse()
//         {
//             Id = updatedRole.Id,
//             Name = updatedRole.Name,
//             Description = updatedRole.Description,
//         };
//     }

//     public async Task<RoleResponse> RemoveMembersFromRole(int id, List<int> memberIds, CancellationToken cancellationToken = default)
//     {
//         var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
//         if (role == null)
//         {
//             throw new KeyNotFoundException($"Role with id {id} not found");
//         }

//         // Only get members that are actually in the role
//         var existingMemberIds = role.Members.Select(m => m.Id).ToList();
//         var memberIdsToRemove = memberIds.Intersect(existingMemberIds).ToList();

//         if (memberIdsToRemove.Any())
//         {
//             var membersToRemove = await _memberRepository.AsQueryable()
//                 .Where(m => memberIdsToRemove.Contains(m.Id))
//                 .ToListAsync(cancellationToken);

//             role.RemoveMembers(membersToRemove);
//             var updatedRole = await _roleRepository.UpdateAsync(role, cancellationToken);
//             return new RoleResponse()
//             {
//                 Id = updatedRole.Id,
//                 Name = updatedRole.Name,
//                 Description = updatedRole.Description,
//             };
//         }

//         return new RoleResponse()
//         {
//             Id = role.Id,
//             Name = role.Name,
//             Description = role.Description,
//             Color = role.Color,
//             OrganizationId = role.OrganizationId,
//             Members = role.Members.Select(m => new MemberResponse() { Id = m.Id, FirstName = m.FirstName, LastName = m.LastName }).ToList(),
//             Permissions = role.Permissions.Select(p => new PermissionResponse() { Id = p.Id, Name = p.Name }).ToList()
//         };
//     }
// }
