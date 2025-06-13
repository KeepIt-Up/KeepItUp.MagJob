using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;

/// <summary>
/// Handler for the GetOrganizationInvitationsQuery.
/// </summary>
public class GetOrganizationInvitationsQueryHandler : IRequestHandler<GetOrganizationInvitationsQuery, Result<PaginationResult<InvitationDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetOrganizationInvitationsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrganizationInvitationsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetOrganizationInvitationsQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetOrganizationInvitationsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetOrganizationInvitationsQuery.
    /// </summary>
    /// <param name="request">GetOrganizationInvitationsQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of invitations to the organization with pagination.</returns>
    public async Task<Result<PaginationResult<InvitationDto>>> Handle(GetOrganizationInvitationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _repository.ExistsAsync(request.OrganizationId, cancellationToken))
            {
                return Result<PaginationResult<InvitationDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // bool hasAccess = await _repository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);

            // if (!hasAccess)
            // {
            //     return Result<PaginationResult<InvitationDto>>.Forbidden("Brak dostępu do organizacji.");
            // }

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
                CreatedBy = Guid.Empty // TODO: Tymczasowa wartość domyślna
            };

            Expression<Func<Invitation, bool>> filter = i => i.Status == InvitationStatus.Pending;

            var paginationResult = await _repository.GetInvitationsByOrganizationIdWithPaginationAsync(
                request.OrganizationId,
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
