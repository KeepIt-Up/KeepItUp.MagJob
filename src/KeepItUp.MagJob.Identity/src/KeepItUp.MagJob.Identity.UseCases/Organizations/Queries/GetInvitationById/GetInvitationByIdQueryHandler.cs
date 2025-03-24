using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetInvitationById;

/// <summary>
/// Handler dla zapytania GetInvitationByIdQuery.
/// </summary>
public class GetInvitationByIdQueryHandler : IRequestHandler<GetInvitationByIdQuery, Result<InvitationDto>>
{
    private readonly IReadRepository<Organization> _repository;
    private readonly ILogger<GetInvitationByIdQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetInvitationByIdQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetInvitationByIdQueryHandler(
        IReadRepository<Organization> repository,
        ILogger<GetInvitationByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetInvitationByIdQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetInvitationByIdQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Zaproszenie do organizacji.</returns>
    public async Task<Result<InvitationDto>> Handle(GetInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.FirstOrDefaultAsync(
                new OrganizationWithInvitationsSpec(request.OrganizationId), cancellationToken);

            if (organization == null)
            {
                return Result<InvitationDto>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma dostęp do organizacji
            bool hasAccess = organization.OwnerId == request.UserId ||
                             organization.Members.Any(m => m.UserId == request.UserId);

            if (!hasAccess)
            {
                return Result<InvitationDto>.Forbidden("Brak dostępu do organizacji.");
            }

            // Znajdź zaproszenie
            var invitation = organization.Invitations.FirstOrDefault(i => i.Id == request.InvitationId);
            if (invitation == null)
            {
                return Result<InvitationDto>.NotFound($"Nie znaleziono zaproszenia o ID {request.InvitationId} w organizacji.");
            }

            // Utwórz DTO zaproszenia
            var invitationDto = new InvitationDto
            {
                Id = invitation.Id,
                OrganizationId = organization.Id,
                Email = invitation.Email,
                Token = invitation.Token,
                Status = invitation.Status.ToString(),
                ExpiresAt = invitation.ExpiresAt,
                IsExpired = DateTime.UtcNow > invitation.ExpiresAt,
                CreatedAt = invitation.CreatedAt,
                CreatedBy = organization.OwnerId // Tymczasowo przypisujemy właściciela organizacji jako twórcę zaproszenia
            };

            return Result<InvitationDto>.Success(invitationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania zaproszenia o ID {InvitationId} w organizacji o ID {OrganizationId}", 
                request.InvitationId, request.OrganizationId);
            return Result<InvitationDto>.Error("Wystąpił błąd podczas pobierania zaproszenia: " + ex.Message);
        }
    }
} 
