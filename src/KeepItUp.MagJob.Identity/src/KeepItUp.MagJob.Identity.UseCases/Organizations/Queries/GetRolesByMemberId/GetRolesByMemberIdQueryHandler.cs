using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByMemberId;

/// <summary>
/// Handler dla zapytania GetRolesByMemberIdQuery.
/// </summary>
public class GetRolesByMemberIdQueryHandler : IRequestHandler<GetRolesByMemberIdQuery, Result<List<RoleDto>>>
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
    /// <returns>Lista ról przypisanych do członka organizacji.</returns>
    public async Task<Result<List<RoleDto>>> Handle(GetRolesByMemberIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<List<RoleDto>>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik wykonujący zapytanie ma dostęp do organizacji
            bool hasAccess = organization.OwnerId == request.RequestingUserId ||
                             await _repository.HasMemberAsync(request.OrganizationId, request.RequestingUserId, cancellationToken);

            if (!hasAccess)
            {
                return Result<List<RoleDto>>.Forbidden("Brak dostępu do organizacji.");
            }

            // Znajdź członka organizacji
            var member = organization.Members.FirstOrDefault(m => m.UserId == request.MemberUserId);
            if (member == null)
            {
                return Result<List<RoleDto>>.NotFound($"Nie znaleziono członka o ID użytkownika {request.MemberUserId} w organizacji.");
            }

            // Pobierz role przypisane do członka
            var roleIds = member.RoleIds;
            var roles = organization.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Color = r.Color,
                    Permissions = r.Permissions.Select(p => p.Name).ToList()
                })
                .ToList();

            return Result<List<RoleDto>>.Success(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania ról członka o ID {MemberUserId} w organizacji o ID {OrganizationId}",
                request.MemberUserId, request.OrganizationId);
            return Result<List<RoleDto>>.Error("Wystąpił błąd podczas pobierania ról członka: " + ex.Message);
        }
    }
}
