using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;

/// <summary>
/// Handler for the RejectInvitationCommand.
/// </summary>
public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, Result>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<RejectInvitationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RejectInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public RejectInvitationCommandHandler(
        IOrganizationRepository organizationRepository,
        ILogger<RejectInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
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
            var organization = await _organizationRepository.GetByIdWithInvitationsAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            organization.RejectInvitation(request.InvitationId);

            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Odrzucono zaproszenie {InvitationId} do organizacji {OrganizationId}",
                request.InvitationId, request.OrganizationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odrzucania zaproszenia {InvitationId} do organizacji {OrganizationId}",
                request.InvitationId, request.OrganizationId);
            return Result.Error("Wystąpił błąd podczas odrzucania zaproszenia: " + ex.Message);
        }
    }
}
