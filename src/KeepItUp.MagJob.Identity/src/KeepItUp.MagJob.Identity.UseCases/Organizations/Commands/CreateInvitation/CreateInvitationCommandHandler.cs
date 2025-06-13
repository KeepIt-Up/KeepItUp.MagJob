using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;

/// <summary>
/// Handler for the CreateInvitationCommand.
/// </summary>
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<CreateInvitationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public CreateInvitationCommandHandler(
        IOrganizationRepository organizationRepository,
        ILogger<CreateInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateInvitationCommand.
    /// </summary>
    /// <param name="request">CreateInvitationCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of the created invitation.</returns>
    public async Task<Result<Guid>> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _organizationRepository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            var invitation = organization.CreateInvitation(request.Email, request.RoleId);

            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Utworzono zaproszenie o ID {InvitationId} dla adresu e-mail {Email} do organizacji {OrganizationId}",
                invitation.Id, request.Email, organization.Id);

            return Result<Guid>.Success(invitation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia zaproszenia");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia zaproszenia: " + ex.Message);
        }
    }
}
