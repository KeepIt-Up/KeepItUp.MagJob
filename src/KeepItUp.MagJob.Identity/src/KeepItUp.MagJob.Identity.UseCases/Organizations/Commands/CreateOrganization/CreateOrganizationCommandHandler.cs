using Ardalis.Result;
using Ardalis.SharedKernel;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Specifications;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

/// <summary>
/// Handler dla komendy CreateOrganizationCommand.
/// </summary>
public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    private readonly IRepository<Organization> _organizationRepository;
    private readonly IReadRepository<User> _userRepository;
    private readonly ILogger<CreateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="CreateOrganizationCommandHandler"/>.
    /// </summary>
    /// <param name="organizationRepository">Repozytorium organizacji.</param>
    /// <param name="userRepository">Repozytorium użytkowników.</param>
    /// <param name="logger">Logger.</param>
    public CreateOrganizationCommandHandler(
        IRepository<Organization> organizationRepository,
        IReadRepository<User> userRepository,
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
            // Sprawdź, czy właściciel istnieje
            var owner = await _userRepository.FirstOrDefaultAsync(
                new UserByIdSpec(request.OwnerId), cancellationToken);

            if (owner == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono użytkownika o ID {request.OwnerId}.");
            }

            // Sprawdź, czy organizacja o podanej nazwie już istnieje
            var existingOrganization = await _organizationRepository.FirstOrDefaultAsync(
                new OrganizationByNameSpec(request.Name), cancellationToken);

            if (existingOrganization != null)
            {
                return Result<Guid>.Error("Organizacja o podanej nazwie już istnieje.");
            }

            // Utwórz nową organizację
            var organization = Organization.Create(
                request.Name,
                request.OwnerId,
                request.Description);

            // Zapisz organizację w repozytorium
            await _organizationRepository.AddAsync(organization, cancellationToken);
            await _organizationRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utworzono nową organizację o ID {OrganizationId}", organization.Id);

            return Result<Guid>.Success(organization.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia organizacji");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia organizacji: " + ex.Message);
        }
    }
} 
