using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;

/// <summary>
/// Handler dla zapytania GetPermissionsQuery.
/// </summary>
public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<List<PermissionDto>>>
{
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetPermissionsQueryHandler"/>.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public GetPermissionsQueryHandler(ILogger<GetPermissionsQueryHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetPermissionsQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetPermissionsQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista wszystkich dostępnych uprawnień w systemie.</returns>
    public Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz wszystkie dostępne uprawnienia z systemu
            var permissions = new List<PermissionDto>
            {
                // Uprawnienia dla organizacji
                new PermissionDto { Name = Permission.StandardPermissions.ManageOrganization.Name, Description = "Zarządzanie organizacją", Category = "Organizacja" },
                new PermissionDto { Name = Permission.StandardPermissions.ViewOrganization.Name, Description = "Przeglądanie organizacji", Category = "Organizacja" },
                
                // Uprawnienia dla członków
                new PermissionDto { Name = Permission.StandardPermissions.ManageMembers.Name, Description = "Zarządzanie członkami organizacji", Category = "Członkowie" },
                new PermissionDto { Name = Permission.StandardPermissions.ViewMembers.Name, Description = "Przeglądanie członków organizacji", Category = "Członkowie" },
                
                // Uprawnienia dla ról
                new PermissionDto { Name = Permission.StandardPermissions.ManageRoles.Name, Description = "Zarządzanie rolami w organizacji", Category = "Role" },
                new PermissionDto { Name = Permission.StandardPermissions.ViewRoles.Name, Description = "Przeglądanie ról w organizacji", Category = "Role" },
                
                // Uprawnienia dla zaproszeń
                new PermissionDto { Name = Permission.StandardPermissions.ManageInvitations.Name, Description = "Zarządzanie zaproszeniami do organizacji", Category = "Zaproszenia" },
                new PermissionDto { Name = Permission.StandardPermissions.ViewInvitations.Name, Description = "Przeglądanie zaproszeń do organizacji", Category = "Zaproszenia" },
                
                // Dodatkowe uprawnienia dla projektów
                new PermissionDto { Name = "projects.manage", Description = "Zarządzanie projektami", Category = "Projekty" },
                new PermissionDto { Name = "projects.view", Description = "Przeglądanie projektów", Category = "Projekty" },
                new PermissionDto { Name = "projects.create", Description = "Tworzenie projektów", Category = "Projekty" },
                
                // Dodatkowe uprawnienia dla zadań
                new PermissionDto { Name = "tasks.manage", Description = "Zarządzanie zadaniami", Category = "Zadania" },
                new PermissionDto { Name = "tasks.view", Description = "Przeglądanie zadań", Category = "Zadania" },
                new PermissionDto { Name = "tasks.create", Description = "Tworzenie zadań", Category = "Zadania" },
                new PermissionDto { Name = "tasks.assign", Description = "Przydzielanie zadań", Category = "Zadania" },
                
                // Dodatkowe uprawnienia dla raportów
                new PermissionDto { Name = "reports.view", Description = "Przeglądanie raportów", Category = "Raporty" },
                new PermissionDto { Name = "reports.create", Description = "Tworzenie raportów", Category = "Raporty" }
            };

            return Task.FromResult(Result<List<PermissionDto>>.Success(permissions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania uprawnień");
            return Task.FromResult(Result<List<PermissionDto>>.Error("Wystąpił błąd podczas pobierania uprawnień: " + ex.Message));
        }
    }
}
