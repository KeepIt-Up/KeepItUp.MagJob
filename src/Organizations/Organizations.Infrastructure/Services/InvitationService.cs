// using Microsoft.EntityFrameworkCore;
// using Organizations.API.Common.Accessors;
// using Organizations.API.Common.Models;
// using Organizations.API.Common.Services;
// using Organizations.API.Repositories;
// using Organizations.Domain.Entities;

// namespace Organizations.API.Services;

// public interface IInvitationService : IBaseService<Invitation>
// {
//     Task<CreateInvitationResponse> CreateInvitationAsync(int organizationId, string userId);
//     Task<PaginatedList<Invitation>> GetInvitationsByOrganizationIdAsync(int organizationId, PaginationOptions pagination);
//     Task<Invitation> AcceptInvitationAsync(int id);
//     Task<Invitation> RejectInvitationAsync(int id);
//     Task<Invitation> CancelInvitationAsync(int id);
//     Task<List<InvitationResponse>> GetInvitationsByUserIdAsync(string userId);
// }

// public class InvitationService : BaseService<Invitation>, IInvitationService
// {
//     private readonly new IInvitationRepository _repository;
//     private readonly IUserAccessor _userAccessor;
//     private readonly IOrganizationService _organizationService;
//     private readonly IMemberRepository _memberRepository;

//     public InvitationService(IInvitationRepository repository, IUserAccessor userAccessor, IOrganizationService organizationService, IMemberRepository memberRepository) : base(repository)
//     {
//         _repository = repository ?? throw new ArgumentNullException(nameof(repository));
//         _userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
//         _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
//         _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
//     }

//     public async Task<CreateInvitationResponse> CreateInvitationAsync(int organizationId, string userId)
//     {
//         //Validation
//         //user exists
//         //TODO: implement user existence check
//         //organization exists
//         var organization = await _organizationService.GetByIdAsync(organizationId)
//             ?? throw new KeyNotFoundException($"Organization with ID {organizationId} was not found.");
//         //user is not already a member of the organization
//         if (organization.IsMember(userId))
//             throw new InvalidOperationException($"User with ID {userId} is already a member of organization with ID {organizationId}.");
//         //user is not already invited to the organization
//         if (organization.IsInvited(userId))
//             throw new InvalidOperationException($"User with ID {userId} is already invited to organization with ID {organizationId}.");

//         //create invitation
//         var invitation = Invitation.Create(organizationId, userId);

//         //send notification to user
//         await _repository.CreateAsync(invitation);

//         return new CreateInvitationResponse
//         {
//             Id = invitation.Id,
//             UserId = userId,
//             OrganizationId = organizationId,
//             OrganizationName = organization.Name,
//             UserName = ""
//         };
//     }

//     public async Task<Invitation> AcceptInvitationAsync(int id)
//     {
//         var invitation = await _repository.AsQueryable().Include(i => i.Organization).ThenInclude(o => o.Members).SingleAsync(i => i.Id == id);

//         var member = invitation.Accept();

//         // add user to organization
//         await _memberRepository.CreateAsync(member);

//         //TODO: send notification to organization members

//         await _repository.UpdateAsync(invitation);

//         return invitation;
//     }

//     public async Task<Invitation> RejectInvitationAsync(int id)
//     {
//         var invitation = await _repository.GetByIdAsync(id)
//             ?? throw new KeyNotFoundException($"Invitation with ID {id} was not found.");

//         invitation.Reject();
//         await _repository.UpdateAsync(invitation);
//         return invitation;
//     }

//     public async Task<Invitation> CancelInvitationAsync(int id)
//     {
//         var invitation = await _repository.GetByIdAsync(id)
//             ?? throw new KeyNotFoundException($"Invitation with ID {id} was not found.");

//         invitation.Status = InvitationStatus.Cancelled;
//         await _repository.UpdateAsync(invitation);
//         return invitation;
//     }

//     public async Task<List<InvitationResponse>> GetInvitationsByUserIdAsync(string userId)
//     {
//         var invitations = await _repository.AsQueryable().Where(i => i.UserId == userId).Include(i => i.Organization).ToListAsync();

//         return invitations.Select(i => new InvitationResponse
//         {
//             Id = i.Id,
//             CreatedAt = i.CreatedAt,
//             Status = i.Status,
//             OrganizationName = i.Organization.Name,
//             OrganizationId = i.OrganizationId.ToString(),
//             UserName = "",
//             UserId = i.UserId
//         }).ToList();
//     }

//     public async Task<PaginatedList<Invitation>> GetInvitationsByOrganizationIdAsync(int organizationId, PaginationOptions pagination)
//     {
//         var query = _repository.AsQueryable()
//             .Where(i => i.OrganizationId == organizationId);

//         return await PaginatedList<Invitation>.CreateAsync(query, pagination);
//     }
// }
