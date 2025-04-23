using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;

/// <summary>
/// Handler dla zapytania GetOrganizationInvitationsQuery.
/// </summary>
public class GetOrganizationInvitationsQueryHandler : IRequestHandler<GetOrganizationInvitationsQuery, Result<PaginationResult<InvitationDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetOrganizationInvitationsQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationInvitationsQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetOrganizationInvitationsQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetOrganizationInvitationsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetOrganizationInvitationsQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetOrganizationInvitationsQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista zaproszeń do organizacji z paginacją.</returns>
    public async Task<Result<PaginationResult<InvitationDto>>> Handle(GetOrganizationInvitationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź czy organizacja istnieje
            if (!await _repository.ExistsAsync(request.OrganizationId, cancellationToken))
            {
                return Result<PaginationResult<InvitationDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // // Sprawdź, czy użytkownik wykonujący zapytanie ma dostęp do organizacji
            // bool hasAccess = await _repository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);

            // if (!hasAccess)
            // {
            //     return Result<PaginationResult<InvitationDto>>.Forbidden("Brak dostępu do organizacji.");
            // }

            // Definiujemy selektor do mapowania Invitation na InvitationDto
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
                CreatedBy = Guid.Empty // Tymczasowa wartość domyślna
            };

            // Definiujemy filtr na Status.Pending
            Expression<Func<Invitation, bool>> filter = i => i.Status == InvitationStatus.Pending;

            // Używamy repozytorium z paginacją
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
