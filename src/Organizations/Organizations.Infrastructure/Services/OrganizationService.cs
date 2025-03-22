// using Organizations.API.Common.Accessors;
// using Organizations.API.Common.Models;
// using Organizations.API.Common.Services;
// using Organizations.API.Models;
// using Organizations.API.Repositories;
// using Microsoft.EntityFrameworkCore;
// using Organizations.Domain.Entities;

// namespace Organizations.API.Services;


// public interface IOrganizationService : IBaseService<Organization>
// {
//     Task<GetOrganizationDto> CreateOrganizationAsync(string name, string? description);
//     Task<GetOrganizationDto> UpdateOrganizationAsync(int id, UpdateOrganizationRequest request);
//     Task<PaginatedList<Organization>> GetOrganizationsByUserIdAsync(string userId, PaginationOptions pagination);
//     Task<List<Organization>> GetByUserIdAsync(string userId);
//     Task<PaginatedList<Organization>> GetByUserIdAsync(string userId, PaginationOptions pagination);
//     Task<Invitation> InviteUserAsync(int id, string userId);
//     Task<PaginatedList<Invitation>> GetInvitationsAsync(int organizationId, PaginationOptions pagination);
//     Task<PaginatedList<Member>> GetMembersAsync(int organizationId, PaginationOptions pagination);
//     Task<Member> RemoveMemberAsync(int id, string memberId);
//     Task<Role> AddRoleAsync(int id, string name, string? description, string? color);
//     Task<PaginatedList<RoleResponse>> GetRolesAsync(int organizationId, PaginationOptions pagination);
//     Task<Role> RemoveRoleAsync(int id, int roleId);
// }

// public class OrganizationService : BaseService<Organization>, IOrganizationService
// {
//     private readonly new IOrganizationRepository _repository;
//     private readonly IUserAccessor _userAccessor;

//     public OrganizationService(IOrganizationRepository repository, IUserAccessor userAccessor) : base(repository)
//     {
//         _repository = repository ?? throw new ArgumentNullException(nameof(repository));
//         _userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
//     }

//     public async Task<GetOrganizationDto> CreateOrganizationAsync(string name, string? description)
//     {
//         //check user exists
//         //var user = await _userRepository.GetByIdAsync(_userAccessor.GetUserId()) ?? throw new KeyNotFoundException("User with id " + _userAccessor.GetUserId() + " not found");

//         var organization = Organization.Create(name, _userAccessor.GetUserId(), description);
//         await _repository.CreateAsync(organization);

//         return new GetOrganizationDto()
//         {
//             Id = organization.Id,
//             Name = organization.Name,
//             Description = organization.Description,
//             Archived = organization.Archived,
//             OwnerId = organization.OwnerId,
//             ProfileImage = organization.ProfileImage,
//             BannerImage = organization.BannerImage
//         };
//     }

//     public async Task<GetOrganizationDto> UpdateOrganizationAsync(int id, UpdateOrganizationRequest request)
//     {
//         var organization = await _repository.GetByIdAsync(id);
//         organization.Update(request.Name, request.Description, request.ProfileImage, request.BannerImage);
//         await _repository.UpdateAsync(organization);
//         return new GetOrganizationDto()
//         {
//             Id = organization.Id,
//             Name = organization.Name,
//             Description = organization.Description,
//             Archived = organization.Archived,
//             OwnerId = organization.OwnerId,
//             ProfileImage = organization.ProfileImage,
//             BannerImage = organization.BannerImage
//         };
//     }

//     public async Task<PaginatedList<Organization>> GetOrganizationsByUserIdAsync(string userId, PaginationOptions pagination)
//     {
//         var query = _repository.AsQueryable().Where(org => org.Members.Any(m => m.UserId == userId));
//         return await PaginatedList<Organization>.CreateAsync(query, pagination);
//     }

//     public async Task<List<Organization>> GetByUserIdAsync(string userId)
//     {
//         return await _repository.AsQueryable().Where(org => org.Members.Any(m => m.UserId == userId)).ToListAsync();
//     }

//     public async Task<PaginatedList<Organization>> GetByUserIdAsync(string userId, PaginationOptions pagination)
//     {
//         var query = _repository.AsQueryable().Where(org => org.Members.Any(m => m.UserId == userId));
//         return await PaginatedList<Organization>.CreateAsync(query, pagination);
//     }

//     public async Task<Invitation> InviteUserAsync(int id, string userId)
//     {
//         //check user exists
//         //var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User with id " + userId + " not found");

//         //check organization exists
//         var organization = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Organization with id " + id + " not found");

//         var invitation = organization.InviteUser(userId);

//         await _repository.UpdateAsync(organization);

//         //send invitations
//         //await _notificationService.SendInvitationAsync(invitation);

//         return invitation;
//     }

//     public async Task<PaginatedList<Invitation>> GetInvitationsAsync(int organizationId, PaginationOptions pagination)
//     {
//         var query = _repository.AsQueryable().Where(org => org.Id == organizationId).SelectMany(org => org.Invitations);
//         return await PaginatedList<Invitation>.CreateAsync(query, pagination);
//     }

//     public async Task<PaginatedList<Member>> GetMembersAsync(int organizationId, PaginationOptions pagination)
//     {
//         var query = _repository.AsQueryable().Where(org => org.Id == organizationId).SelectMany(org => org.Members);
//         return await PaginatedList<Member>.CreateAsync(query, pagination);
//     }

//     public async Task<Member> RemoveMemberAsync(int id, string memberId)
//     {
//         var organization = await _repository.GetByIdAsync(id);
//         return organization.RemoveMember(memberId);
//     }

//     public async Task<Role> AddRoleAsync(int id, string name, string? description, string? color)
//     {
//         var organization = await _repository.GetByIdAsync(id);
//         return organization.AddRole(name, description, color);
//     }

//     public async Task<PaginatedList<RoleResponse>> GetRolesAsync(int organizationId, PaginationOptions pagination)
//     {
//         var query = _repository.AsQueryable().Where(org => org.Id == organizationId).SelectMany(org => org.Roles)
//         .Select(r => new RoleResponse()
//         {
//             Id = r.Id,
//             Name = r.Name,
//             Description = r.Description,
//             Color = r.Color,
//             OrganizationId = r.OrganizationId,
//             Members = r.Members.Select(m => new MemberResponse() { Id = m.Id, FirstName = m.FirstName, LastName = m.LastName }).ToList(),
//             Permissions = r.Permissions.Select(p => new PermissionResponse() { Id = p.Id, Name = p.Name }).ToList()
//         });
//         return await PaginatedList<RoleResponse>.CreateAsync(query, pagination);
//     }

//     public async Task<Role> RemoveRoleAsync(int id, int roleId)
//     {
//         var organization = await _repository.GetByIdAsync(id);
//         return organization.RemoveRole(roleId);
//     }
// }