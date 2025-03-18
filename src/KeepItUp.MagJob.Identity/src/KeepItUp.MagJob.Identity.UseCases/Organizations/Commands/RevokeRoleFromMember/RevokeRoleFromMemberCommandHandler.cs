using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;

/// <summary>
/// Handler dla komendy RevokeRoleFromMemberCommand.
/// </summary>
public class RevokeRoleFromMemberCommandHandler : IRequestHandler<RevokeRoleFromMemberCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<RevokeRoleFromMemberCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RevokeRoleFromMemberCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public RevokeRoleFromMemberCommandHandler(
        IOrganizationRepository repository,
        ILogger<RevokeRoleFromMemberCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę RevokeRoleFromMemberCommand.
    /// </summary>
    /// <param name="request">Komenda RevokeRoleFromMemberCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(RevokeRoleFromMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do odbierania ról
            if (organization.OwnerId != request.RequestingUserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.RequestingUserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do odbierania ról w organizacji.");
                }
            }

            // Sprawdź, czy rola istnieje w organizacji
            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            // Sprawdź, czy użytkownik jest członkiem organizacji
            var member = organization.Members.FirstOrDefault(m => m.UserId == request.MemberUserId);
            if (member == null)
            {
                return Result.NotFound($"Użytkownik o ID {request.MemberUserId} nie jest członkiem organizacji.");
            }

            // Sprawdź, czy użytkownik ma przypisaną tę rolę
            if (!member.HasRole(request.RoleId))
            {
                return Result.Error($"Użytkownik o ID {request.MemberUserId} nie ma przypisanej roli o ID {request.RoleId}.");
            }

            // Sprawdź, czy to nie jest ostatnia rola użytkownika
            if (member.Roles.Count == 1)
            {
                return Result.Error("Nie można odebrać ostatniej roli użytkownikowi. Użytkownik musi mieć przypisaną co najmniej jedną rolę.");
            }

            // Odbierz rolę członkowi organizacji
            member.RemoveRole(request.RoleId);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Odebrano rolę o ID {RoleId} użytkownikowi o ID {UserId} w organizacji o ID {OrganizationId}",
                request.RoleId, request.MemberUserId, organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odbierania roli");
            return Result.Error("Wystąpił błąd podczas odbierania roli: " + ex.Message);
        }
    }
}
