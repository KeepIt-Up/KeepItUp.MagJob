using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;

/// <summary>
/// Handler for the GetOrganizationMembersQuery.
/// </summary>
public class GetOrganizationMembersQueryHandler : IRequestHandler<GetOrganizationMembersQuery, Result<PaginationResult<MemberDto>>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetOrganizationMembersQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationMembersQueryHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public GetOrganizationMembersQueryHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        ILogger<GetOrganizationMembersQueryHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetOrganizationMembersQuery.
    /// </summary>
    /// <param name="request">GetOrganizationMembersQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of members of the organization with pagination.</returns>
    public async Task<Result<PaginationResult<MemberDto>>> Handle(GetOrganizationMembersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _organizationRepository.ExistsAsync(request.OrganizationId, cancellationToken))
            {
                return Result<PaginationResult<MemberDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            //bool hasAccess = await _organizationRepository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);

            //if (!hasAccess)
            //{
            //    return Result<PaginationResult<MemberDto>>.Forbidden("Brak dostępu do organizacji.");
            //}

            Expression<Func<Core.OrganizationAggregate.Member, MemberDto>> memberSelector = member => new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                Email = "", // TODO: Te pola będą uzupełnione po pobraniu danych
                FirstName = "", // TODO: Te pola będą uzupełnione po pobraniu danych
                LastName = "", // TODO: Te pola będą uzupełnione po pobraniu danych
                Roles = member.Roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Color = r.Color,
                    Permissions = r.Permissions.Select(p => p.Name).ToList()
                }).ToList()
            };

            var paginationResult = await _organizationRepository.GetMembersByOrganizationIdWithPaginationAsync(
                request.OrganizationId,
                memberSelector,
                request.PaginationParameters,
                cancellationToken);

            var userIds = paginationResult.Items.Select(m => m.UserId).ToList();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

            foreach (var memberDto in paginationResult.Items)
            {
                var user = users.FirstOrDefault(u => u.Id == memberDto.UserId);
                if (user != null)
                {
                    memberDto.Email = user.Email;
                    memberDto.FirstName = user.FirstName;
                    memberDto.LastName = user.LastName;
                }
            }

            return Result<PaginationResult<MemberDto>>.Success(paginationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania członków organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<PaginationResult<MemberDto>>.Error("Wystąpił błąd podczas pobierania członków organizacji: " + ex.Message);
        }
    }
}
