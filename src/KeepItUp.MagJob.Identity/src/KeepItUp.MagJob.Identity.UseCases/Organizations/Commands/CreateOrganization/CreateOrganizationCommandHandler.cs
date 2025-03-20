using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

/// <summary>
/// Handler dla komendy CreateOrganizationCommand.
/// </summary>
public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateOrganizationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public CreateOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        ILogger<CreateOrganizationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obsługuje komendę CreateOrganizationCommand.
    /// </summary>
    /// <param name="request">Komenda CreateOrganizationCommand.</param>
    /// <param name="cancellationToken">Token anulowania.</param>
    /// <returns>Identyfikator utworzonej organizacji.</returns>
    public async Task<Result<Guid>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź, czy użytkownik istnieje
            var user = await _userRepository.GetByExternalIdAsync(request.OwnerId, cancellationToken);
            if (user == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono użytkownika o ID {request.OwnerId}.");
            }

            // Sprawdź, czy nazwa organizacji jest unikalna
            var existingOrganization = await _organizationRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingOrganization != null)
            {
                return Result<Guid>.Error($"Organizacja o nazwie '{request.Name}' już istnieje.");
            }

            // Utwórz nową organizację
            var organization = Organization.Create(
                request.Name,
                user.Id, // użyj ID użytkownika zamiast ExternalId
                request.Description,
                logoUrl: null,
                bannerUrl: null);

            // Zapisz organizację w repozytorium, aby uzyskać prawidłowe ID
            await _organizationRepository.AddAsync(organization, cancellationToken);

            // Inicjalizuj role i członkostwo właściciela i ponownie zapisz
            organization.InitializeRoles();
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            // Inicjalizuj członkostwo właściciela
            organization.InitializeOwner();
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Utworzono nową organizację {OrganizationId} dla użytkownika {UserId}",
                organization.Id, user.Id);

            return Result<Guid>.Success(organization.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia organizacji dla użytkownika {UserId}", request.OwnerId);
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia organizacji: " + ex.Message);
        }
    }
}
