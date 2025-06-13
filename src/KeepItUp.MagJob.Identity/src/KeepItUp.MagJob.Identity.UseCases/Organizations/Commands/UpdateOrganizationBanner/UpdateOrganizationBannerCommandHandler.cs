using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;

/// <summary>
/// Handler for the UpdateOrganizationBannerCommand.
/// </summary>
public class UpdateOrganizationBannerCommandHandler : IRequestHandler<UpdateOrganizationBannerCommand, Result>
{
    private readonly IOrganizationRepository _repository;
    private readonly ILogger<UpdateOrganizationBannerCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationBannerCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">Organization repository.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationBannerCommandHandler(
        IOrganizationRepository repository,
        ILogger<UpdateOrganizationBannerCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateOrganizationBannerCommand.
    /// </summary>
    /// <param name="request">UpdateOrganizationBannerCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    public async Task<Result> Handle(UpdateOrganizationBannerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _repository.GetByIdAsync(request.OrganizationId, cancellationToken);
            if (organization == null)
            {
                return Result.NotFound($"Nie znaleziono organizacji o ID {request.OrganizationId}.");
            }

            organization.UpdateBanner(request.BannerUrl);

            await _repository.UpdateAsync(organization, cancellationToken);

            _logger.LogInformation("Zaktualizowano banner organizacji o ID {OrganizationId}", organization.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji bannera organizacji");
            return Result.Error("Wystąpił błąd podczas aktualizacji bannera organizacji: " + ex.Message);
        }
    }
}
