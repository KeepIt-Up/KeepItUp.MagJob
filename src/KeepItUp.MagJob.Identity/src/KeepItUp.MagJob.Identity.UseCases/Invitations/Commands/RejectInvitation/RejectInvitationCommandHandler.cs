using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Commands.RejectInvitation;

/// <summary>
/// Handler for the RejectInvitationCommand.
/// </summary>
public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, Result>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly ILogger<RejectInvitationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RejectInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="invitationRepository">Invitation repository.</param>
    /// <param name="logger">Logger.</param>
    public RejectInvitationCommandHandler(
        IInvitationRepository invitationRepository,
        ILogger<RejectInvitationCommandHandler> logger)
    {
        _invitationRepository = invitationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the RejectInvitationCommand.
    /// </summary>
    /// <param name="request">RejectInvitationCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result> Handle(RejectInvitationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);

            if (invitation == null)
            {
                return Result.NotFound($"Nie znaleziono zaproszenia o ID {request.InvitationId}.");
            }

            invitation.Reject();

            await _invitationRepository.UpdateAsync(invitation, cancellationToken);

            _logger.LogInformation("Odrzucono zaproszenie {InvitationId} do organizacji {OrganizationId}",
                request.InvitationId, invitation.OrganizationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odrzucania zaproszenia {InvitationId}",
                request.InvitationId);
            return Result.Error("Wystąpił błąd podczas odrzucania zaproszenia: " + ex.Message);
        }
    }
}