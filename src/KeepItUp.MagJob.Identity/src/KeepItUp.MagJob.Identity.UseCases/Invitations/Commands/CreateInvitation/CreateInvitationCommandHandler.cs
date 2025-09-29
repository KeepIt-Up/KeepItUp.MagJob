using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.InvitationAggregate;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
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
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateInvitationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="invitationRepository">Invitation repository.</param>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public CreateInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        ILogger<CreateInvitationCommandHandler> logger)
    {
        _invitationRepository = invitationRepository;
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
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
            var user = await _userRepository.GetByExternalIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono użytkownika o ID {request.UserId}.");
            }

            var organization = await _organizationRepository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);
            if (organization == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            if (!organization.HasAccess(user.Id))
            {
                return Result<Guid>.Forbidden("Brak uprawnień do zapraszania użytkowników do tej organizacji.");
            }

            if (!organization.HasRole(request.RoleId))
            {
                return Result<Guid>.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji {request.OrganizationId}.");
            }

            var hasPendingInvitation = await _invitationRepository.HasPendingInvitationAsync(request.OrganizationId, request.Email, cancellationToken);
            if (hasPendingInvitation)
            {
                return Result<Guid>.Error($"Zaproszenie dla adresu {request.Email} do organizacji już istnieje.");
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