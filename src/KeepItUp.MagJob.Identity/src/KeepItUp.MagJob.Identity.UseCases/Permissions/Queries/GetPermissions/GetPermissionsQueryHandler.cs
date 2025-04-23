using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Permissions.Queries.GetPermissions;

/// <summary>
/// Handler dla zapytania GetPermissionsQuery.
/// </summary>
public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<PaginationResult<PermissionDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetPermissionsQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetPermissionsQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetPermissionsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetPermissionsQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetPermissionsQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista wszystkich dostępnych uprawnień w systemie z paginacją.</returns>
    public async Task<Result<PaginationResult<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Definiujemy selektor do mapowania Permission na PermissionDto
            Expression<Func<Permission, PermissionDto>> selector = p => new PermissionDto
            {
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Category = DetermineCategory(p.Name)
            };

            // Pobieramy uprawnienia z paginacją używając repozytorium
            var paginationResult = await _repository.GetPermissionsWithPaginationAsync(
                selector,
                request.PaginationParameters,
                cancellationToken);

            return Result<PaginationResult<PermissionDto>>.Success(paginationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania uprawnień");
            return Result<PaginationResult<PermissionDto>>.Error("Wystąpił błąd podczas pobierania uprawnień: " + ex.Message);
        }
    }

    /// <summary>
    /// Określa kategorię uprawnienia na podstawie jego nazwy.
    /// </summary>
    /// <param name="permissionName">Nazwa uprawnienia.</param>
    /// <returns>Kategoria uprawnienia.</returns>
    private static string DetermineCategory(string permissionName)
    {
        if (permissionName.StartsWith("organization"))
            return "Organizacja";
        if (permissionName.StartsWith("members"))
            return "Członkowie";
        if (permissionName.StartsWith("roles"))
            return "Role";
        if (permissionName.StartsWith("invitations"))
            return "Zaproszenia";
        if (permissionName.StartsWith("projects"))
            return "Projekty";
        if (permissionName.StartsWith("tasks"))
            return "Zadania";
        if (permissionName.StartsWith("reports"))
            return "Raporty";

        return "Inne";
    }
}
