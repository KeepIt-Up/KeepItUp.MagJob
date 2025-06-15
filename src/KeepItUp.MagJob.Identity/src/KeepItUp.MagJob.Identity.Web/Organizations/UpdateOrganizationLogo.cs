using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;
using FluentValidation;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to update the logo of an organization.
/// </summary>
public class UpdateOrganizationLogo : BaseEndpoint<UpdateOrganizationLogoRequest, UpdateOrganizationLogoResponse>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<UpdateOrganizationLogo> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationLogo"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="currentUserAccessor">Current user accessor.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationLogo(
        IMediator mediator,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<UpdateOrganizationLogo> logger)
    {
        _mediator = mediator;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override void ConfigureEndpoint()
    {
        Put(UpdateOrganizationLogoRequest.Route);
        AllowFileUploads();
        AllowFormData();
        Summary(s =>
        {
            s.Summary = "Updates the logo of an organization";
            s.Description = "Updates the logo of an organization with the specified identifier";
        });
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{organizationId}/logo request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    protected override async Task<UpdateOrganizationLogoResponse> HandleEndpointAsync(UpdateOrganizationLogoRequest req, CancellationToken ct)
    {
        var currentUserId = _currentUserAccessor.GetRequiredCurrentUserId();

        if (req.LogoFile == null || req.LogoFile.Length == 0)
        {
            throw new ValidationException("Logo file is required");
        }

        var command = new UpdateOrganizationLogoCommand
        {
            OrganizationId = req.OrganizationId,
            LogoFile = req.LogoFile,
            UserId = currentUserId
        };

        var result = await _mediator.Send(command, ct);

        return new UpdateOrganizationLogoResponse { LogoUrl = result };
    }
}
