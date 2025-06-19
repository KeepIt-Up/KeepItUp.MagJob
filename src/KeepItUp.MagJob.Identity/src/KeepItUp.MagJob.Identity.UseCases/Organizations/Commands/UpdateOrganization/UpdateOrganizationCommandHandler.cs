using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Handler for the UpdateOrganizationCommand.
/// </summary>
public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, Result<EmptyResponse>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateOrganizationCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateOrganizationCommand.
    /// </summary>
    /// <param name="request">UpdateOrganizationCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result<EmptyResponse>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdWithMembersAndRolesAsync(request.Id, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.Id}.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result.Error("Nazwa organizacji jest wymagana.");
            }

            if (organization.OwnerId != request.UserId)
            {
                var isMember = organization.Members.Any(m => m.UserId == request.UserId &&
                    m.Roles.Any(r => r.Name == "Admin"));

                if (!isMember)
                {
                    return Result.Forbidden("Brak uprawnień do aktualizacji tej organizacji.");
                }
            }

            if (organization.Name != request.Name)
            {
                var existingOrganization = await _repository.GetByNameAsync(request.Name, cancellationToken);

                if (existingOrganization != null && existingOrganization.Id != request.Id)
                {
                    return Result.Error("Organizacja o podanej nazwie już istnieje.");
                }
            }

            organization.Update(request.Name, request.Description);

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano organizację o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji organizacji");
            return Result.Error("Wystąpił błąd podczas aktualizacji organizacji: " + ex.Message);
        }
    }
}
