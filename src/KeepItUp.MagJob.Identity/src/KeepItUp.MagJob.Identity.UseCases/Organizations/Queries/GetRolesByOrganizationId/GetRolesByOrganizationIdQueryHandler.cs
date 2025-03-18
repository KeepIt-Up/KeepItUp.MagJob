using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;

/// <summary>
/// Handler dla zapytania GetRolesByOrganizationIdQuery.
/// </summary>
public class GetRolesByOrganizationIdQueryHandler : IRequestHandler<GetRolesByOrganizationIdQuery, Result<List<RoleDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetRolesByOrganizationIdQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetRolesByOrganizationIdQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetRolesByOrganizationIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetRolesByOrganizationIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetRolesByOrganizationIdQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetRolesByOrganizationIdQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista ról w organizacji.</returns>
    public async Task<Result<List<RoleDto>>> Handle(GetRolesByOrganizationIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<List<RoleDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma dostęp do organizacji
            bool hasAccess = organization.OwnerId == request.UserId ||
                             organization.Members.Any(m => m.UserId == request.UserId);

            if (!hasAccess)
            {
                return Result<List<RoleDto>>.Forbidden("Brak dostępu do organizacji.");
            }

            // Mapuj role na DTO
            var roles = organization.Roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Color = r.Color,
                Permissions = r.Permissions.Select(p => p.Name).ToList()
            }).ToList();

            return Result<List<RoleDto>>.Success(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ról organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<List<RoleDto>>.Error("Wystąpił błąd podczas pobierania ról organizacji: " + ex.Message);
        }
    }
}
