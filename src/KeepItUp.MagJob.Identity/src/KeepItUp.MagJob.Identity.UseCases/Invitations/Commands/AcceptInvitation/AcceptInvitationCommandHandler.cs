using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.AcceptInvitation;

/// <summary>
/// Handler for the AcceptInvitationCommand.
/// </summary>
public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Result<EmptyResponse>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AcceptInvitationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="invitationRepository">Invitation repository.</param>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public AcceptInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        ILogger<AcceptInvitationCommandHandler> logger)
    {
        _invitationRepository = invitationRepository;
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the AcceptInvitationCommand.
    /// </summary>
    /// <param name="request">AcceptInvitationCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Member identifier.</returns>
    public async Task<Result<EmptyResponse>> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);

            if (invitation == null)
            {
                return Result<EmptyResponse>.NotFound($"Invitation with ID {request.InvitationId} not found.");
            }

            invitation.Accept(request.Token);

            await _invitationRepository.UpdateAsync(invitation, cancellationToken);

            // Verify that user exists (for logging purposes)
            var user = await _userRepository.GetByEmailAsync(invitation.Email, cancellationToken);
            if (user == null)
            {
                return Result<EmptyResponse>.Error($"User with email {invitation.Email} not found.");
            }

            // Verify that organization exists (for logging purposes)
            var organization = await _organizationRepository.GetByIdAsync(invitation.OrganizationId, cancellationToken);
            if (organization == null)
            {
                return Result<EmptyResponse>.Error($"Organization with ID {invitation.OrganizationId} not found.");
            }

            // Note: Member creation is handled by InvitationAcceptedEventHandler

            _logger.LogInformation("Użytkownik {UserId} zaakceptował zaproszenie {InvitationId} do organizacji {OrganizationId}",
                request.UserId, request.InvitationId, invitation.OrganizationId);

            return Result<EmptyResponse>.Success(new EmptyResponse());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas akceptowania zaproszenia {InvitationId} przez użytkownika {UserId}",
                request.InvitationId, request.UserId);
            return Result<EmptyResponse>.Error("Wystąpił błąd podczas akceptowania zaproszenia: " + ex.Message);
        }
    }
}