using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;

/// <summary>
/// Handler for the AcceptInvitationCommand.
/// </summary>
public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<AcceptInvitationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptInvitationCommandHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public AcceptInvitationCommandHandler(
        IOrganizationRepository organizationRepository,
        ILogger<AcceptInvitationCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
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
            // Get the organization with invitations
            var organization = await _organizationRepository.GetByIdWithInvitationsAsync(request.OrganizationId, cancellationToken);

            // Validator should ensure that the organization exists
            if (organization == null)
            {
                return Result<Guid>.NotFound($"Organization with ID {request.OrganizationId} not found.");
            }

            // Accept the invitation and add the user as a member of the organization
            var member = organization.AcceptInvitation(request.InvitationId, request.UserId);

            // Save changes
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Użytkownik {UserId} zaakceptował zaproszenie {InvitationId} do organizacji {OrganizationId}",
                request.UserId, request.InvitationId, request.OrganizationId);

            return Result<Guid>.Success(member.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas akceptowania zaproszenia {InvitationId} przez użytkownika {UserId} do organizacji {OrganizationId}",
                request.InvitationId, request.UserId, request.OrganizationId);
            return Result<Guid>.Error("Wystąpił błąd podczas akceptowania zaproszenia: " + ex.Message);
        }
    }
}
