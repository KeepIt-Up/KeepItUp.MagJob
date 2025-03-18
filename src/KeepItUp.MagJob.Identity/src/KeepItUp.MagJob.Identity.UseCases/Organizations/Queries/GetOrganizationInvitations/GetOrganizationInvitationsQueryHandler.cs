using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;

/// <summary>
/// Handler dla zapytania GetOrganizationInvitationsQuery.
/// </summary>
public class GetOrganizationInvitationsQueryHandler : IRequestHandler<GetOrganizationInvitationsQuery, Result<List<InvitationDto>>>
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
    /// <returns>Lista zaproszeń do organizacji.</returns>
    public async Task<Result<List<InvitationDto>>> Handle(GetOrganizationInvitationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<List<InvitationDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik wykonujący zapytanie ma dostęp do organizacji
            bool hasAccess = organization.OwnerId == request.UserId ||
                              await _repository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);

            if (!hasAccess)
            {
                return Result<List<InvitationDto>>.Forbidden("Brak dostępu do organizacji.");
            }

            // Mapuj zaproszenia na DTO
            var invitationDtos = organization.Invitations
                .Where(i => i.Status == InvitationStatus.Pending) // Pobierz tylko oczekujące zaproszenia
                .Select(i => new InvitationDto
                {
                    Id = i.Id,
                    Email = i.Email,
                    CreatedAt = i.CreatedAt,
                    ExpiresAt = i.ExpiresAt,
                    Status = i.Status.ToString()
                })
                .ToList();

            return Result<List<InvitationDto>>.Success(invitationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania zaproszeń dla organizacji o ID {OrganizationId}",
                request.OrganizationId);
            return Result<List<InvitationDto>>.Error("Wystąpił błąd podczas pobierania zaproszeń: " + ex.Message);
        }
    }
}
