using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;

/// <summary>
/// Handler dla zapytania GetOrganizationByIdQuery.
/// </summary>
public class GetOrganizationByIdQueryHandler : IRequestHandler<GetOrganizationByIdQuery, Result<OrganizationDto>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetOrganizationByIdQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetOrganizationByIdQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetOrganizationByIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetOrganizationByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetOrganizationByIdQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetOrganizationByIdQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Dane organizacji.</returns>
    public async Task<Result<OrganizationDto>> Handle(GetOrganizationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<OrganizationDto>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma dostęp do organizacji
            //bool hasAccess = organization.OwnerId == request.UserId ||
            //                 organization.Members.Any(m => m.UserId == request.UserId);

            //if (!hasAccess)
            //{
            //    return Result<OrganizationDto>.Forbidden("Brak dostępu do organizacji.");
            //}

            // Pobierz role użytkownika w organizacji
            var userRoles = new List<string>();
            var member = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
            if (member != null)
            {
                userRoles = member.Roles.Select(r => r.Name).ToList();
            }
            else if (organization.OwnerId == request.UserId)
            {
                // Właściciel organizacji ma wszystkie role
                userRoles = organization.Roles.Select(r => r.Name).ToList();
            }

            // Mapuj organizację na DTO
            var organizationDto = new OrganizationDto
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                OwnerId = organization.OwnerId,
                IsActive = organization.IsActive,
                UserRoles = userRoles
            };

            return Result<OrganizationDto>.Success(organizationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<OrganizationDto>.Error("Wystąpił błąd podczas pobierania organizacji: " + ex.Message);
        }
    }
}
