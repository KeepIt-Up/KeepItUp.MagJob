using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.DeactivateOrganization;

/// <summary>
/// Handler for the DeactivateOrganizationCommand.
/// </summary>
public class DeactivateOrganizationCommandHandler : IRequestHandler<DeactivateOrganizationCommand, Result<EmptyResponse>>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<DeactivateOrganizationCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateOrganizationCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public DeactivateOrganizationCommandHandler(
        IOrganizationRepository repository,
        ILogger<DeactivateOrganizationCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeactivateOrganizationCommand.
    /// </summary>
    /// <param name="request">DeactivateOrganizationCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result<EmptyResponse>> Handle(DeactivateOrganizationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.Id}.");
            }

            if (organization.OwnerId != request.UserId)
            {
                return Result.Forbidden("Tylko właściciel organizacji może ją dezaktywować.");
            }

            organization.Deactivate();

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Dezaktywowano organizację o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas dezaktywacji organizacji");
            return Result.Error("Wystąpił błąd podczas dezaktywacji organizacji: " + ex.Message);
        }
    }
}
