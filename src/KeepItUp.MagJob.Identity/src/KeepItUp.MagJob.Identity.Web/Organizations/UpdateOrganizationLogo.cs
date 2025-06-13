using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to update the logo of an organization.
/// </summary>
public class UpdateOrganizationLogo : Endpoint<UpdateOrganizationLogoRequest, UpdateOrganizationLogoResponse>
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<UpdateOrganizationLogo> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationLogo"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="fileStorageService">File storage service.</param>
    /// <param name="currentUserAccessor">Current user accessor.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationLogo(
        IMediator mediator,
        IFileStorageService fileStorageService,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<UpdateOrganizationLogo> logger)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override void Configure()
    {
        Put(UpdateOrganizationLogoRequest.Route);
        AllowFileUploads();
        AllowFormData();
        Description(d =>
        {
            d.WithName("UpdateOrganizationLogo");
            d.WithTags("Organizations");
            d.WithSummary("Updates the logo of an organization");
            d.WithDescription("Updates the logo of an organization.");
        });
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{organizationId}/logo request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    public override async Task HandleAsync(UpdateOrganizationLogoRequest req, CancellationToken ct)
    {
        var currentUserId = _currentUserAccessor.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            AddError("Użytkownik niezalogowany");
            await SendErrorsAsync(StatusCodes.Status401Unauthorized, ct);
            return;
        }

        var getOrganizationQuery = new GetOrganizationByIdQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = currentUserId.Value
        };

        var organizationResult = await _mediator.Send(getOrganizationQuery, ct);

        if (!organizationResult.IsSuccess)
        {
            if (organizationResult.Status == ResultStatus.NotFound)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (organizationResult.Status == ResultStatus.Forbidden)
            {
                await SendForbiddenAsync(ct);
                return;
            }

            foreach (var error in organizationResult.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        string? oldLogoUrl = organizationResult.Value.LogoUrl;

        try
        {
            string logoUrl;
            using (var stream = req.LogoFile!.OpenReadStream())
            {
                logoUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    req.LogoFile.FileName,
                    req.LogoFile.ContentType,
                    "logos"
                );
            }

            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = req.OrganizationId,
                LogoUrl = logoUrl,
                UserId = currentUserId.Value
            };

            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(oldLogoUrl) && oldLogoUrl != logoUrl)
                {
                    await _fileStorageService.DeleteFileAsync(oldLogoUrl);
                }

                await SendAsync(new UpdateOrganizationLogoResponse { LogoUrl = logoUrl }, StatusCodes.Status200OK, ct);
                return;
            }

            await _fileStorageService.DeleteFileAsync(logoUrl);

            foreach (var error in result.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji logo organizacji {OrganizationId}", req.OrganizationId);
            AddError("Wystąpił błąd podczas przetwarzania pliku");
            await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
        }
    }
}
