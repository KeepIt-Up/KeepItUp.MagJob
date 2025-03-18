using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateRole;

/// <summary>
/// Handler dla komendy CreateRoleCommand.
/// </summary>
public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateRoleCommandHandler"/>.
    /// </summary>
    /// <param name="repository">Repozytorium organizacji.</param>
    /// <param name="logger">Logger.</param>
    public CreateRoleCommandHandler(
        IOrganizationRepository repository,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę CreateRoleCommand.
    /// </summary>
    /// <param name="request">Komenda CreateRoleCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Identyfikator utworzonej roli.</returns>
    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Pobierz organizację z repozytorium
            var organization = await _repository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            // Sprawdź, czy użytkownik ma uprawnienia do tworzenia ról
            if (organization.OwnerId != request.UserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result<Guid>.Forbidden("Brak uprawnień do tworzenia ról w organizacji.");
                }
            }

            // Sprawdź, czy rola o podanej nazwie już istnieje w organizacji
            if (organization.Roles.Any(r => r.Name == request.Name))
            {
                return Result<Guid>.Error($"Rola o nazwie '{request.Name}' już istnieje w organizacji.");
            }

            // Utwórz nową rolę
            var role = organization.AddRole(
                request.Name,
                request.Description,
                request.Color ?? "#CCCCCC");

            // Zapisz zmiany w repozytorium
            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Utworzono nową rolę o ID {RoleId} w organizacji o ID {OrganizationId}",
                role.Id, organization.Id);

            return Result<Guid>.Success(role.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia roli");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia roli: " + ex.Message);
        }
    }
}
