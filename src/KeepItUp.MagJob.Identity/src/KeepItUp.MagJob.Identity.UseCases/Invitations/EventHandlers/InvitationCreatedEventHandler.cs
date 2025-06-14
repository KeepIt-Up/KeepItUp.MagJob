using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Events;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.EventHandlers;

/// <summary>
/// Handler for the InvitationCreatedEvent.
/// Sends an email invitation to the invited user.
/// </summary>
public class InvitationCreatedEventHandler : INotificationHandler<InvitationCreatedEvent>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<InvitationCreatedEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvitationCreatedEventHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="emailSender">Email sender service.</param>
    /// <param name="logger">Logger.</param>
    public InvitationCreatedEventHandler(
        IOrganizationRepository organizationRepository,
        IEmailSender emailSender,
        ILogger<InvitationCreatedEventHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _emailSender = emailSender;
        _logger = logger;
    }

    /// <summary>
    /// Handles the InvitationCreatedEvent.
    /// </summary>
    /// <param name="notification">The invitation created event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task Handle(InvitationCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _organizationRepository.GetByIdAsync(notification.OrganizationId, cancellationToken);

            if (organization == null)
            {
                _logger.LogWarning("Nie znaleziono organizacji {OrganizationId} dla zaproszenia {InvitationId}",
                    notification.OrganizationId, notification.InvitationId);
                return;
            }

            var role = organization.Roles.FirstOrDefault(r => r.Id == notification.RoleId);

            if (role == null)
            {
                _logger.LogWarning("Nie znaleziono roli {RoleId} w organizacji {OrganizationId} dla zaproszenia {InvitationId}",
                    notification.RoleId, notification.OrganizationId, notification.InvitationId);
                return;
            }

            var subject = $"Zaproszenie do organizacji {organization.Name}";
            var body = GenerateInvitationEmailBody(organization.Name, role.Name, notification.InvitationId);

            await _emailSender.SendEmailAsync(
                notification.Email,
                subject,
                body);

            _logger.LogInformation("Wysłano email z zaproszeniem {InvitationId} do organizacji {OrganizationId} na adres {Email}",
                notification.InvitationId, notification.OrganizationId, notification.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wysyłania emaila z zaproszeniem {InvitationId} na adres {Email}",
                notification.InvitationId, notification.Email);
        }
    }

    /// <summary>
    /// Generates the HTML body for the invitation email.
    /// </summary>
    /// <param name="organizationName">Name of the organization.</param>
    /// <param name="roleName">Name of the role.</param>
    /// <param name="invitationId">Invitation ID for acceptance link.</param>
    /// <returns>HTML email body.</returns>
    private string GenerateInvitationEmailBody(string organizationName, string roleName, Guid invitationId)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Zaproszenie do organizacji</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Zaproszenie do organizacji</h1>
        </div>
        <div class='content'>
            <h2>Witaj!</h2>
            <p>Zostałeś/aś zaproszony/a do dołączenia do organizacji <strong>{organizationName}</strong> w roli <strong>{roleName}</strong>.</p>
            
            <p>Aby zaakceptować zaproszenie, kliknij poniższy przycisk:</p>
            
            <a href='#' class='button'>Zaakceptuj zaproszenie</a>
            
            <p>Lub skopiuj i wklej poniższy link do przeglądarki:</p>
            <p style='word-break: break-all; background-color: #fff; padding: 10px; border: 1px solid #ddd;'>
                [LINK_DO_AKCEPTACJI]
            </p>
            
            <p><strong>ID zaproszenia:</strong> {invitationId}</p>
            
            <p>Jeśli nie spodziewałeś/aś się tego zaproszenia, możesz zignorować tę wiadomość.</p>
        </div>
        <div class='footer'>
            <p>Ta wiadomość została wysłana automatycznie przez system MagJob.</p>
        </div>
    </div>
</body>
</html>";
    }
}