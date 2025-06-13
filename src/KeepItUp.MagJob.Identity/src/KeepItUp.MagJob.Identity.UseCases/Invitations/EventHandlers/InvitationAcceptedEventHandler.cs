using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Events;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.EventHandlers;

/// <summary>
/// Handler for the InvitationAcceptedEvent.
/// </summary>
public class InvitationAcceptedEventHandler : INotificationHandler<InvitationAcceptedEvent>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<InvitationAcceptedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvitationAcceptedEventHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="userRepository">User repository.</param>
    /// <param name="logger">Logger.</param>
    public InvitationAcceptedEventHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        ILogger<InvitationAcceptedEventHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the InvitationAcceptedEvent.
    /// </summary>
    /// <param name="notification">The invitation accepted event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task Handle(InvitationAcceptedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(notification.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Nie znaleziono użytkownika o adresie email {Email} dla zaakceptowanego zaproszenia {InvitationId}",
                    notification.Email, notification.InvitationId);
                return;
            }

            var organization = await _organizationRepository.GetByIdAsync(notification.OrganizationId, cancellationToken);

            if (organization == null)
            {
                _logger.LogWarning("Nie znaleziono organizacji {OrganizationId} dla zaakceptowanego zaproszenia {InvitationId}",
                    notification.OrganizationId, notification.InvitationId);
                return;
            }

            var member = organization.AddMemberFromInvitation(user.Id, notification.RoleId);

            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Dodano użytkownika {UserId} jako członka {MemberId} organizacji {OrganizationId} po zaakceptowaniu zaproszenia {InvitationId}",
                user.Id, member.Id, notification.OrganizationId, notification.InvitationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas obsługi InvitationAcceptedEvent dla zaproszenia {InvitationId}",
                notification.InvitationId);
            throw;
        }
    }
}