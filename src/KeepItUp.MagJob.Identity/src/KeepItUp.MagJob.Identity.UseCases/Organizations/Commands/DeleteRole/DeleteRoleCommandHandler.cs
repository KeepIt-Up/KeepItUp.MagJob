using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeleteRole;

/// <summary>
/// Handler for the DeleteRoleCommand.
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<DeleteRoleCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteRoleCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public DeleteRoleCommandHandler(
        IOrganizationRepository repository,
        ILogger<DeleteRoleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeleteRoleCommand.
    /// </summary>
    /// <param name="request">DeleteRoleCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            var role = organization.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
            {
                return Result.NotFound($"Nie znaleziono roli o ID {request.RoleId} w organizacji.");
            }

            if (role.Name is "Admin" or "Member" or "Guest")
            {
                return Result.Error("Nie można usunąć domyślnej roli systemowej.");
            }

            var membersWithRole = organization.Members.Where(m => m.HasRole(request.RoleId)).ToList();
            if (membersWithRole.Any())
            {
                return Result.Error("Nie można usunąć roli, która jest przypisana do członków organizacji.");
            }

            await _repository.DeleteRoleAsync(request.OrganizationId, request.RoleId, cancellationToken);

            _logger.LogInformation("Usunięto rolę o ID {RoleId} z organizacji o ID {OrganizationId}",
                request.RoleId, request.OrganizationId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas usuwania roli");
            return Result.Error("Wystąpił błąd podczas usuwania roli: " + ex.Message);
        }
    }
}
