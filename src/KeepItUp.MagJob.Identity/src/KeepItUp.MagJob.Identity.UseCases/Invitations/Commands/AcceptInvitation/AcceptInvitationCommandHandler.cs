using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.AcceptInvitation;

/// <summary>
/// Handler for the AcceptInvitationCommand.
/// </summary>
public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Result<Guid>>
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
    public async Task<Result<Guid>> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);

            if (invitation == null)
            {
                return Result<Guid>.NotFound($"Invitation with ID {request.InvitationId} not found.");
            }

            invitation.Accept();

            await _invitationRepository.UpdateAsync(invitation, cancellationToken);

            var user = await _userRepository.GetByEmailAsync(invitation.Email, cancellationToken);
            if (user == null)
            {
                return Result<Guid>.Error($"User with email {invitation.Email} not found.");
            }

            var organization = await _organizationRepository.GetByIdWithMembersAsync(invitation.OrganizationId, cancellationToken);
            if (organization == null)
            {
                return Result<Guid>.Error($"Organization with ID {invitation.OrganizationId} not found.");
            }

            var member = organization.Members.FirstOrDefault(m => m.UserId == user.Id);
            if (member == null)
            {
                return Result<Guid>.Error("Member was not created successfully.");
            }

            _logger.LogInformation("Użytkownik {UserId} zaakceptował zaproszenie {InvitationId} do organizacji {OrganizationId}, utworzono członka {MemberId}",
                request.UserId, request.InvitationId, invitation.OrganizationId, member.Id);

            return Result<Guid>.Success(member.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas akceptowania zaproszenia {InvitationId} przez użytkownika {UserId}",
                request.InvitationId, request.UserId);
            return Result<Guid>.Error("Wystąpił błąd podczas akceptowania zaproszenia: " + ex.Message);
        }
    }
}