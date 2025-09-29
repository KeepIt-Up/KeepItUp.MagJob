using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

/// <summary>
/// Handler for the CreateOrganizationCommand.
/// </summary>
public class CreateOrganizationCommandHandler : IRequestHandler<CreateOrganizationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOrganizationCommandHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
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
    /// Handles the CreateOrganizationCommand.
    /// </summary>
    /// <param name="request">CreateOrganizationCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of the created organization.</returns>
    public async Task<Result<Guid>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByExternalIdAsync(request.OwnerId, cancellationToken);
            if (user == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono użytkownika o ID {request.OwnerId}.");
            }

            var existingOrganization = await _organizationRepository.GetByNameAsync(request.Name, cancellationToken);
            if (existingOrganization != null)
            {
                return Result<Guid>.Error($"Organizacja o nazwie '{request.Name}' już istnieje.");
            }

            var organization = Organization.Create(
                request.Name,
                user.Id,
                request.Description,
                logoUrl: null,
                bannerUrl: null);

            organization.InitializeRoles();
            organization.InitializeOwner();

            await _organizationRepository.AddAsync(organization, cancellationToken);

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
