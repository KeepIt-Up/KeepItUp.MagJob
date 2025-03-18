using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetMemberById;

/// <summary>
/// Handler dla zapytania GetMemberByIdQuery.
/// </summary>
public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, Result<MemberDto>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<GetMemberByIdQueryHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="GetMemberByIdQueryHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public GetMemberByIdQueryHandler(
        IOrganizationRepository repository,
        ILogger<GetMemberByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje zapytanie GetMemberByIdQuery.
    /// </summary>
    /// <param name="request">Zapytanie GetMemberByIdQuery.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Członek organizacji.</returns>
    public async Task<Result<MemberDto>> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<MemberDto>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik wykonujący zapytanie ma dostęp do organizacji
            bool hasAccess = organization.OwnerId == request.RequestingUserId ||
                             await _repository.HasMemberAsync(request.OrganizationId, request.RequestingUserId, cancellationToken);

            if (!hasAccess)
            {
                return Result<MemberDto>.Forbidden("Brak dostępu do organizacji.");
            }

            // Znajdź członka organizacji
            var member = organization.Members.FirstOrDefault(m => m.UserId == request.MemberUserId);
            if (member == null)
            {
                return Result<MemberDto>.NotFound($"Nie znaleziono członka o ID użytkownika {request.MemberUserId} w organizacji.");
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

            // Utwórz DTO członka
            var memberDto = new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                Email = "user@example.com", // Tymczasowo ustawiam wartość domyślną
                FirstName = "Imię", // Tymczasowo ustawiam wartość domyślną
                LastName = "Nazwisko", // Tymczasowo ustawiam wartość domyślną
                DisplayName = "Użytkownik", // Tymczasowo ustawiam wartość domyślną
                JoinedAt = member.JoinedAt,
                Roles = roles
            };

            return Result<MemberDto>.Success(memberDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania członka o ID {MemberUserId} w organizacji o ID {OrganizationId}",
                request.MemberUserId, request.OrganizationId);
            return Result<MemberDto>.Error("Wystąpił błąd podczas pobierania członka: " + ex.Message);
        }
    }
}
