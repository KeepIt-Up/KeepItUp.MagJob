using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;
using FluentValidation;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to update the banner of an organization.
/// </summary>
public class UpdateOrganizationBanner : BaseEndpoint<UpdateOrganizationBannerRequest, UpdateOrganizationBannerResponse>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<UpdateOrganizationBanner> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationBanner"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="currentUserAccessor">Current user accessor.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationBanner(
        IMediator mediator,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<UpdateOrganizationBanner> logger)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override void ConfigureEndpoint()
    {
        Put(UpdateOrganizationBannerRequest.Route);
        AllowFileUploads();
        AllowFormData();
        Summary(s =>
        {
            s.Summary = "Updates the banner of an organization";
            s.Description = "Updates the banner of an organization with the specified identifier";
        });
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{organizationId}/banner request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    protected override async Task<UpdateOrganizationBannerResponse> HandleEndpointAsync(UpdateOrganizationBannerRequest req, CancellationToken ct)
    {
        var currentUserId = _currentUserAccessor.GetRequiredCurrentUserId();

        if (req.BannerFile == null || req.BannerFile.Length == 0)
        {
            throw new ValidationException("Banner file is required");
        }

        var command = new UpdateOrganizationBannerCommand
        {
            OrganizationId = req.OrganizationId,
            BannerFile = req.BannerFile,
            UserId = currentUserId
        };

        var result = await _mediator.Send(command, ct);

        return new UpdateOrganizationBannerResponse { BannerUrl = result };
    }
}
