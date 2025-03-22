using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRoleById;

/// <summary>
/// Handler dla zapytania GetRoleByIdQuery.
/// </summary>
public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly IReadRepository<Organization> _repository;
    private readonly ILogger<GetRoleByIdQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetRoleByIdQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetRoleByIdQueryHandler(
        IReadRepository<Organization> repository,
        ILogger<GetRoleByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetRoleByIdQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetRoleByIdQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Dane roli.</returns>
    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.FirstOrDefaultAsync(
                new OrganizationWithRolesSpec(request.OrganizationId), cancellationToken);

            if (organization == null)
            {
                return Result<RoleDto>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma dostęp do organizacji
            bool hasAccess = organization.OwnerId == request.UserId ||
                             organization.Members.Any(m => m.UserId == request.UserId);

            if (!hasAccess)
            {
                return Result<RoleDto>.Forbidden("Brak dostępu do organizacji.");
            }

            // Znajdź rolę
            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result<RoleDto>.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            // Mapuj rolę na DTO
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Color = role.Color,
                Permissions = role.Permissions.Select(p => p.Name).ToList()
            };

            return Result<RoleDto>.Success(roleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania roli o ID {RoleId} w organizacji o ID {OrganizationId}",
                request.RoleId, request.OrganizationId);
            return Result<RoleDto>.Error("Wystąpił błąd podczas pobierania roli: " + ex.Message);
        }
    }
} 
