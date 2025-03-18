using Ardalis.Result;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;

/// <summary>
/// Handler dla komendy AssignRoleToMemberCommand.
/// </summary>
public class AssignRoleToMemberCommandHandler : IRequestHandler<AssignRoleToMemberCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<AssignRoleToMemberCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="AssignRoleToMemberCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public AssignRoleToMemberCommandHandler(
        IOrganizationRepository repository,
        ILogger<AssignRoleToMemberCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę AssignRoleToMemberCommand.
    /// </summary>
    /// <param name="request">Komenda AssignRoleToMemberCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(AssignRoleToMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do przypisywania ról
            if (organization.OwnerId != request.RequestingUserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.RequestingUserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do przypisywania ról w organizacji.");
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

            // Sprawdź, czy użytkownik już ma przypisaną tę rolę
            if (member.HasRole(request.RoleId))
            {
                return Result.Error($"Użytkownik o ID {request.MemberUserId} już ma przypisaną rolę o ID {request.RoleId}.");
            }

            // Przypisz rolę członkowi organizacji
            member.AssignRole(request.RoleId);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Przypisano rolę o ID {RoleId} użytkownikowi o ID {UserId} w organizacji o ID {OrganizationId}",
                request.RoleId, request.MemberUserId, organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas przypisywania roli");
            return Result.Error("Wystąpił błąd podczas przypisywania roli: " + ex.Message);
        }
    }
}
