using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.CreateInvitation;

/// <summary>
/// Handler for the CreateInvitationCommand.
/// </summary>
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, Result<Guid>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<CreateInvitationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="invitationRepository">Invitation repository.</param>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public CreateInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IOrganizationRepository organizationRepository,
        ILogger<CreateInvitationCommandHandler> logger)
    {
        _invitationRepository = invitationRepository;
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

            if (!organization.HasRole(request.RoleId))
            {
                return Result<Guid>.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji {request.OrganizationId}.");
            }

            var invitation = Invitation.Create(
                request.OrganizationId,
                request.Email,
                request.RoleId);

            await _invitationRepository.AddAsync(invitation, cancellationToken);

            _logger.LogInformation("Utworzono zaproszenie o ID {InvitationId} dla adresu e-mail {Email} do organizacji {OrganizationId}",
                invitation.Id, request.Email, request.OrganizationId);

            return Result<Guid>.Success(invitation.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia zaproszenia");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia zaproszenia: " + ex.Message);
        }
    }
}