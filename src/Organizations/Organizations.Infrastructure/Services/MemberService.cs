// using Microsoft.EntityFrameworkCore;
// using Organizations.API.Common.Accessors;
// using Organizations.API.Common.Models;
// using Organizations.API.Common.Services;
// using Organizations.API.Repositories;
// using Organizations.Domain.Entities;

// namespace Organizations.API.Services;

// public interface IMemberService : IBaseService<Member>
// {
//     Task<PaginatedList<Member>> GetMembersByOrganizationIdAsync(int organizationId, PaginationOptions pagination);
//     Task ArchiveMemberAsync(int id);

//     Task<Member> UpdateMemberAsync(int id, UpdateMemberRequest request);

//     Task<PaginatedList<Member>> GetMembers(string name, int organizationId, PaginationOptions pagination);
// }

// public class MemberService : BaseService<Member>, IMemberService
// {
//     private readonly new IMemberRepository _repository;
//     private readonly IUserAccessor _userAccessor;

//     public MemberService(IMemberRepository repository, IUserAccessor userAccessor) : base(repository)
//     {
//         _repository = repository ?? throw new ArgumentNullException(nameof(repository));
//         _userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
//     }

//     public async Task<PaginatedList<Member>> GetMembersByOrganizationIdAsync(int organizationId, PaginationOptions pagination)
//     {
//         if (pagination == null)
//             throw new ArgumentNullException(nameof(pagination));

//         return await PaginatedList<Member>.CreateAsync(
//             _repository.AsQueryable().Where(m => m.OrganizationId == organizationId),
//             pagination);
//     }

//     public async Task<Member> UpdateMemberAsync(int id, UpdateMemberRequest request)
//     {
//         var member = await _repository.GetByIdAsync(id);

//         member.Update(request);

//         return await _repository.UpdateAsync(member);
//     }

//     public async Task ArchiveMemberAsync(int id)
//     {
//         await _repository.ArchiveMemberAsync(id);
//     }

//     public async Task<PaginatedList<Member>> GetMembers(string name, int organizationId, PaginationOptions pagination)
//     {
//         if (string.IsNullOrWhiteSpace(name))
//             return new PaginatedList<Member>(new List<Member>(), 0, pagination.PageNumber, pagination.PageSize);

//         var query = _repository.AsQueryable()
//             .Where(m => (m.FirstName.ToLower().Contains(name.ToLower()) ||
//                         m.LastName.ToLower().Contains(name.ToLower())) &&
//                         m.OrganizationId == organizationId);

//         return await PaginatedList<Member>.CreateAsync(query, pagination);
//     }
// }