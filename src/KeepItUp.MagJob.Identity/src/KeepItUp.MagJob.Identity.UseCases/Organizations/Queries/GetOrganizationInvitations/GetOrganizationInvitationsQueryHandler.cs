using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationInvitations;

/// <summary>
/// Handler dla zapytania GetOrganizationInvitationsQuery.
/// </summary>
public class GetOrganizationInvitationsQueryHandler : IRequestHandler<GetOrganizationInvitationsQuery, Result<List<InvitationDto>>>
{
    private readonly IReadRepository<Organization> _repository;
    private readonly ILogger<GetOrganizationInvitationsQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationInvitationsQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetOrganizationInvitationsQueryHandler(
        IReadRepository<Organization> repository,
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
            var organization = await _repository.FirstOrDefaultAsync(
                new OrganizationWithInvitationsSpec(request.OrganizationId), cancellationToken);

            if (organization == null)
            {
                return Result<List<InvitationDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do przeglądania zaproszeń
            if (organization.OwnerId != request.UserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result<List<InvitationDto>>.Forbidden("Brak uprawnień do przeglądania zaproszeń organizacji.");
                }
            }

            var result = new List<InvitationDto>();

            // Mapuj zaproszenia na DTO
            foreach (var invitation in organization.Invitations)
            {
                var role = organization.Roles.FirstOrDefault(r => r.Id == invitation.RoleId);
                
                var invitationDto = new InvitationDto
                {
                    Id = invitation.Id,
                    OrganizationId = organization.Id,
                    Email = invitation.Email,
                    Token = invitation.Token,
                    Status = invitation.Status.ToString(),
                    ExpiresAt = invitation.ExpiresAt,
                    IsExpired = invitation.IsExpired,
                    Role = role != null ? new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        Color = role.Color,
                        Permissions = role.Permissions.Select(p => p.Name).ToList()
                    } : null
                };

                result.Add(invitationDto);
            }

            return Result<List<InvitationDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania zaproszeń organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<List<InvitationDto>>.Error("Wystąpił błąd podczas pobierania zaproszeń organizacji: " + ex.Message);
        }
    }
} 
