using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByMemberId;

/// <summary>
/// Handler dla zapytania GetRolesByMemberIdQuery.
/// </summary>
public class GetRolesByMemberIdQueryHandler : IRequestHandler<GetRolesByMemberIdQuery, Result<PaginationResult<RoleDto>>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetRolesByMemberIdQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetRolesByMemberIdQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetRolesByMemberIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetRolesByMemberIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetRolesByMemberIdQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetRolesByMemberIdQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Lista ról przypisanych do członka organizacji z paginacją.</returns>
    public async Task<Result<PaginationResult<RoleDto>>> Handle(GetRolesByMemberIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź czy organizacja istnieje
            if (!await _repository.ExistsAsync(request.OrganizationId, cancellationToken))
            {
                return Result<PaginationResult<RoleDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // // Sprawdź, czy użytkownik wykonujący zapytanie ma dostęp do organizacji
            // bool hasAccess = await _repository.HasMemberAsync(request.OrganizationId, request.RequestingUserId, cancellationToken);

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

            // Pobieramy role członka z paginacją używając repozytorium
            var paginationResult = await _repository.GetRolesByMemberIdWithPaginationAsync(
                request.OrganizationId,
                request.MemberUserId,
                selector,
                request.PaginationParameters,
                cancellationToken);

            return Result<PaginationResult<RoleDto>>.Success(paginationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ról członka o ID {MemberUserId} w organizacji o ID {OrganizationId}",
                request.MemberUserId, request.OrganizationId);
            return Result<PaginationResult<RoleDto>>.Error("Wystąpił błąd podczas pobierania ról członka: " + ex.Message);
        }
    }
}
