using KeepItUp.MagJob.Identity.Core.InvitationAggregate.Repositories;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Invitations.Queries.GetInvitationById;

/// <summary>
/// Handler for the GetInvitationByIdQuery.
/// </summary>
public class GetInvitationByIdQueryHandler : IRequestHandler<GetInvitationByIdQuery, Result<InvitationDto>>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly ILogger<GetInvitationByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInvitationByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="invitationRepository">Invitation repository.</param>
    /// <param name="logger">Logger.</param>
    public GetInvitationByIdQueryHandler(
        IInvitationRepository invitationRepository,
        ILogger<GetInvitationByIdQueryHandler> logger)
    {
        _invitationRepository = invitationRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetInvitationByIdQuery.
    /// </summary>
    /// <param name="request">GetInvitationByIdQuery.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Invitation DTO.</returns>
    public async Task<Result<InvitationDto>> Handle(GetInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);

            if (invitation == null)
            {
                return Result<InvitationDto>.NotFound($"Nie znaleziono zaproszenia o ID {request.InvitationId}.");
            }

            var invitationDto = new InvitationDto
            {
                Id = invitation.Id,
                OrganizationId = invitation.OrganizationId,
                Email = invitation.Email,
                Token = invitation.Token,
                Status = invitation.Status.ToString(),
                ExpiresAt = invitation.ExpiresAt,
                IsExpired = invitation.IsExpired,
                CreatedAt = invitation.CreatedAt,
            };

            return Result<InvitationDto>.Success(invitationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas pobierania zaproszenia {InvitationId}",
                request.InvitationId);
            return Result<InvitationDto>.Error("Wystąpił błąd podczas pobierania zaproszenia: " + ex.Message);
        }
    }
}