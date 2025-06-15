using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint to update the banner of an organization.
/// </summary>
public class UpdateOrganizationBanner : Endpoint<UpdateOrganizationBannerRequest, UpdateOrganizationBannerResponse>
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<UpdateOrganizationBanner> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationBanner"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="fileStorageService">File storage service.</param>
    /// <param name="currentUserAccessor">Current user accessor.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationBanner(
        IMediator mediator,
        IFileStorageService fileStorageService,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<UpdateOrganizationBanner> logger)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override void Configure()
    {
        Put(UpdateOrganizationBannerRequest.Route);
        AllowFileUploads();
        AllowFormData();
    }

    /// <summary>
    /// Handles the PUT /api/organizations/{organizationId}/banner request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    public override async Task HandleAsync(UpdateOrganizationBannerRequest req, CancellationToken ct)
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

        string? oldBannerUrl = organizationResult.Value.BannerUrl;

        try
        {
            string bannerUrl;
            using (var stream = req.BannerFile!.OpenReadStream())
            {
                bannerUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    req.BannerFile.FileName,
                    req.BannerFile.ContentType,
                    "banners"
                );
            }

            var command = new UpdateOrganizationBannerCommand
            {
                OrganizationId = req.OrganizationId,
                BannerUrl = bannerUrl,
                UserId = currentUserId.Value
            };

            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(oldBannerUrl) && oldBannerUrl != bannerUrl)
                {
                    await _fileStorageService.DeleteFileAsync(oldBannerUrl);
                }

                await SendAsync(new UpdateOrganizationBannerResponse { BannerUrl = bannerUrl }, StatusCodes.Status200OK, ct);
                return;
            }

            await _fileStorageService.DeleteFileAsync(bannerUrl);

            foreach (var error in result.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji bannera organizacji {OrganizationId}", req.OrganizationId);
            AddError("Wystąpił błąd podczas przetwarzania pliku");
            await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
        }
    }
}
