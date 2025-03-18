using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;

/// <summary>
/// Handler dla komendy RemoveMemberCommand.
/// </summary>
public class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<RemoveMemberCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="RemoveMemberCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public RemoveMemberCommandHandler(
        IOrganizationRepository repository,
        ILogger<RemoveMemberCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę RemoveMemberCommand.
    /// </summary>
    /// <param name="request">Komenda RemoveMemberCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Wynik operacji.</returns>
    public async Task<Result> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do usuwania członków
            if (organization.OwnerId != request.RequestingUserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.RequestingUserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do usuwania członków organizacji.");
                }
            }

            // Sprawdź, czy użytkownik do usunięcia jest członkiem organizacji
            var memberToRemove = organization.Members.FirstOrDefault(m => m.UserId == request.MemberUserId);
            if (memberToRemove == null)
            {
                return Result.NotFound($"Użytkownik o ID {request.MemberUserId} nie jest członkiem organizacji.");
            }

            // Sprawdź, czy użytkownik do usunięcia nie jest właścicielem organizacji
            if (organization.OwnerId == request.MemberUserId)
            {
                return Result.Error("Nie można usunąć właściciela organizacji.");
            }

            // Usuń członka z organizacji
            organization.RemoveMember(request.MemberUserId);

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Użytkownik o ID {MemberUserId} został usunięty z organizacji o ID {OrganizationId}",
                request.MemberUserId, request.OrganizationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania członka organizacji");
            return Result.Error("Wystąpił błąd podczas usuwania członka organizacji: " + ex.Message);
        }
    }
}
