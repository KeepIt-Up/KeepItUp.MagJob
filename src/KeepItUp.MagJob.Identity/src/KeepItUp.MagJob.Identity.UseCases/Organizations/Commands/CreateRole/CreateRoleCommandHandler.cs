using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateRole;

/// <summary>
/// Handler for the CreateRoleCommand.
/// </summary>
public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateRoleCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public CreateRoleCommandHandler(
        IOrganizationRepository repository,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateRoleCommand.
    /// </summary>
    /// <param name="request">CreateRoleCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of the created role.</returns>
    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithRolesAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<Guid>.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            var role = organization.AddRole(
                request.Name,
                request.Description,
                request.Color ?? "#CCCCCC");

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Utworzono nową rolę o ID {RoleId} w organizacji o ID {OrganizationId}",
                role.Id, organization.Id);

            return Result<Guid>.Success(role.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas tworzenia roli");
            return Result<Guid>.Error("Wystąpił błąd podczas tworzenia roli: " + ex.Message);
        }
    }
}
