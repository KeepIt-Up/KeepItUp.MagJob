using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;

/// <summary>
/// Handler for the RemoveMemberCommand.
/// </summary>
public class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand, Result<EmptyResponse>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<RemoveMemberCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveMemberCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public RemoveMemberCommandHandler(
        IOrganizationRepository repository,
        ILogger<RemoveMemberCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the RemoveMemberCommand.
    /// </summary>
    /// <param name="request">RemoveMemberCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result<EmptyResponse>> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            if (organization.OwnerId != request.RequestingUserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.RequestingUserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do usuwania członków organizacji.");
                }
            }

            var memberToRemove = organization.Members.FirstOrDefault(m => m.UserId == request.MemberUserId);
            if (memberToRemove == null)
            {
                return Result.NotFound($"Użytkownik o ID {request.MemberUserId} nie jest członkiem organizacji.");
            }

            if (organization.OwnerId == request.MemberUserId)
            {
                return Result.Error("Nie można usunąć właściciela organizacji.");
            }

            organization.RemoveMember(request.MemberUserId);

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Użytkownik o ID {MemberUserId} został usunięty z organizacji o ID {OrganizationId}",
                request.MemberUserId, request.OrganizationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania członka organizacji");
            return Result.Error("Wystąpił błąd podczas usuwania członka organizacji: " + ex.Message);
        }
    }
}
