using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByOrganizationId;

/// <summary>
/// Handler dla zapytania GetRolesByOrganizationIdQuery.
/// </summary>
public class GetRolesByOrganizationIdQueryHandler : IRequestHandler<GetRolesByOrganizationIdQuery, Result<PaginationResult<RoleDto>>>
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
    /// <returns>Lista ról w organizacji z paginacją.</returns>
    public async Task<Result<PaginationResult<RoleDto>>> Handle(GetRolesByOrganizationIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź czy organizacja istnieje
            if (!await _repository.ExistsAsync(request.OrganizationId, cancellationToken))
            {
                return Result<PaginationResult<RoleDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // // Sprawdź, czy użytkownik ma dostęp do organizacji
            // bool hasAccess = await _repository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);

            // if (!hasAccess)
            // {
            //     return Result<PaginationResult<RoleDto>>.Forbidden("Brak dostępu do organizacji.");
            // }

            // Definiujemy selektor do mapowania Role na RoleDto
            Expression<Func<Role, RoleDto>> selector = role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Color = role.Color,
                Permissions = role.Permissions.Select(p => p.Name).ToList()
            };

            // Pobieramy role z paginacją używając repozytorium
            var paginationResult = await _repository.GetRolesByOrganizationIdWithPaginationAsync(
                request.OrganizationId,
                selector,
                request.PaginationParameters,
                cancellationToken);

            return Result<PaginationResult<RoleDto>>.Success(paginationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ról organizacji o ID {OrganizationId}", request.OrganizationId);
            return Result<PaginationResult<RoleDto>>.Error("Wystąpił błąd podczas pobierania ról organizacji: " + ex.Message);
        }
    }
}
