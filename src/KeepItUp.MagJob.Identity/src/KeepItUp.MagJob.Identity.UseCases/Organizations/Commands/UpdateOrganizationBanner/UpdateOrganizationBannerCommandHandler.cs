using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;

/// <summary>
/// Handler for the UpdateOrganizationBannerCommand.
/// </summary>
public class UpdateOrganizationBannerCommandHandler : IRequestHandler<UpdateOrganizationBannerCommand, Result<string>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;
    private readonly ILogger<UpdateOrganizationBannerCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationBannerCommandHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="fileStorageService">File storage service.</param>
    /// <param name="fileValidationService">File validation service.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationBannerCommandHandler(
        IOrganizationRepository organizationRepository,
        IFileStorageService fileStorageService,
        IFileValidationService fileValidationService,
        ILogger<UpdateOrganizationBannerCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateOrganizationBannerCommand.
    /// </summary>
    /// <param name="request">UpdateOrganizationBannerCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with the banner URL.</returns>
    public async Task<Result<string>> Handle(UpdateOrganizationBannerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate file
            _fileValidationService.ValidateImageFile(request.BannerFile, "banner");

            // Get organization and validate permissions
            var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<string>.NotFound($"Organization with ID {request.OrganizationId} not found.");
            }

            if (!organization.IsActive)
            {
                return Result<string>.Error("Cannot update banner for inactive organization.");
            }

            // Check if user has permission to update organization banner
            var hasMembership = await _organizationRepository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);
            if (!hasMembership)
            {
                return Result<string>.Forbidden("User does not have permission to update this organization.");
            }

            var oldBannerUrl = organization.BannerUrl;

            // Upload new banner
            string bannerUrl;
            using (var stream = request.BannerFile.OpenReadStream())
            {
                bannerUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    request.BannerFile.FileName,
                    request.BannerFile.ContentType,
                    "banners");
            }

            // Update organization in database
            organization.UpdateBanner(bannerUrl);
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            // Delete old banner if exists and different
            if (!string.IsNullOrEmpty(oldBannerUrl) && oldBannerUrl != bannerUrl)
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(oldBannerUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old banner file {OldBannerUrl}", oldBannerUrl);
                    // Don't fail the operation if we can't delete the old file
                }
            }

            _logger.LogInformation("Successfully updated banner for organization {OrganizationId}", request.OrganizationId);
            return Result.Success(bannerUrl);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating banner for organization {OrganizationId}", request.OrganizationId);
            return Result<string>.Error(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating banner for organization {OrganizationId}", request.OrganizationId);
            return Result<string>.Error("Failed to update organization banner: " + ex.Message);
        }
    }
}
