using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Permissions.Queries.GetPermissions;

/// <summary>
/// Handler for the GetPermissionsQuery.
/// </summary>
public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<PaginationResult<PermissionDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPermissionsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public GetPermissionsQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetPermissionsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetPermissionsQuery.
    /// </summary>
    /// <param name="request">GetPermissionsQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all available permissions in the system with pagination.</returns>
    public async Task<Result<PaginationResult<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Expression<Func<Permission, PermissionDto>> selector = p => new PermissionDto
            {
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                Category = DetermineCategory(p.Name)
            };

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
    /// Determines the category of a permission based on its name.
    /// </summary>
    /// <param name="permissionName">Permission name.</param>
    /// <returns>Permission category.</returns>
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
