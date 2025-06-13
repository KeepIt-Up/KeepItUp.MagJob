using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;

/// <summary>
/// Handler for the UpdateOrganizationLogoCommand.
/// </summary>
public class UpdateOrganizationLogoCommandHandler : IRequestHandler<UpdateOrganizationLogoCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateOrganizationLogoCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationLogoCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationLogoCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateOrganizationLogoCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateOrganizationLogoCommand.
    /// </summary>
    /// <param name="request">UpdateOrganizationLogoCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result> Handle(UpdateOrganizationLogoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdAsync(request.OrganizationId, cancellationToken);
            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            organization.UpdateLogo(request.LogoUrl);

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano logo organizacji o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji logo organizacji");
            return Result.Error("Wystąpił błąd podczas aktualizacji logo organizacji: " + ex.Message);
        }
    }
}
