using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;

/// <summary>
/// Handler for the UpdateRoleCommand.
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<EmptyResponse>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateRoleCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public UpdateRoleCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateRoleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateRoleCommand.
    /// </summary>
    /// <param name="request">UpdateRoleCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result<EmptyResponse>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            if (organization.OwnerId != request.UserId)
            {
                var requestingMember = organization.Members.FirstOrDefault(m => m.UserId == request.UserId);
                if (requestingMember == null || !requestingMember.Roles.Any(r => r.Name == "Admin"))
                {
                    return Result.Forbidden("Brak uprawnień do aktualizacji ról w organizacji.");
                }
            }

            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            if (role.Name is "Admin" or "Member" or "Guest" && role.Name != request.Name)
            {
                return Result.Error("Nie można zmienić nazwy domyślnej roli systemowej.");
            }

            role.Update(
                request.Name,
                request.Description,
                request.Color);

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano rolę o ID {RoleId} w organizacji o ID {OrganizationId}",
                request.RoleId, organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji roli");
            return Result.Error("Wystąpił błąd podczas aktualizacji roli: " + ex.Message);
        }
    }
}
