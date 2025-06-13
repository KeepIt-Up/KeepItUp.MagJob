using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Invitations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Queries.GetInvitations;

/// <summary>
/// Handler for the GetInvitationsQuery.
/// </summary>
public class GetInvitationsQueryHandler : IRequestHandler<GetInvitationsQuery, Result<PaginationResult<InvitationDto>>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetInvitationsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInvitationsQueryHandler"/> class.
    /// </summary>
    /// <param name="invitationRepository">Invitation repository.</param>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetInvitationsQueryHandler(
        IInvitationRepository invitationRepository,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        ILogger<GetInvitationsQueryHandler> logger)
    {
        _invitationRepository = invitationRepository;
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetInvitationsQuery.
    /// </summary>
    /// <param name="request">GetInvitationsQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of invitations for the organization with pagination.</returns>
    public async Task<Result<PaginationResult<InvitationDto>>> Handle(GetInvitationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.OrganizationId != null && !await _organizationRepository.ExistsAsync(request.OrganizationId.Value, cancellationToken))
            {
                return Result<PaginationResult<InvitationDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            Expression<Func<Invitation, InvitationDto>> selector = i => new InvitationDto
            {
                Id = i.Id,
                OrganizationId = i.OrganizationId,
                Email = i.Email,
                Token = i.Token,
                Status = i.Status.ToString(),
                ExpiresAt = i.ExpiresAt,
                IsExpired = i.IsExpired,
                CreatedAt = i.CreatedAt,
            };

            Expression<Func<Invitation, bool>> filter = i =>
                request.OrganizationId != null ? i.OrganizationId == request.OrganizationId : true &&
                request.Email != null ? i.Email == request.Email : true;

            var paginationResult = await _invitationRepository.GetByOrganizationIdWithPaginationAsync(
                selector,
                request.PaginationParameters,
                filter,
                cancellationToken);

            return Result<PaginationResult<InvitationDto>>.Success(paginationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania zaproszeń dla organizacji o ID {OrganizationId}",
                request.OrganizationId);
            return Result<PaginationResult<InvitationDto>>.Error("Wystąpił błąd podczas pobierania zaproszeń: " + ex.Message);
        }
    }
}