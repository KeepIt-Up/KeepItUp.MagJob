using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;

/// <summary>
/// Handler for the UpdateOrganizationLogoCommand.
/// </summary>
public class UpdateOrganizationLogoCommandHandler : IRequestHandler<UpdateOrganizationLogoCommand, Result<string>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;
    private readonly ILogger<UpdateOrganizationLogoCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationLogoCommandHandler"/> class.
    /// </summary>
    /// <param name="organizationRepository">Organization repository.</param>
    /// <param name="fileStorageService">File storage service.</param>
    /// <param name="fileValidationService">File validation service.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationLogoCommandHandler(
        IOrganizationRepository organizationRepository,
        IFileStorageService fileStorageService,
        IFileValidationService fileValidationService,
        ILogger<UpdateOrganizationLogoCommandHandler> logger)
    {
        _organizationRepository = organizationRepository;
        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateOrganizationLogoCommand.
    /// </summary>
    /// <param name="request">UpdateOrganizationLogoCommand.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with the logo URL.</returns>
    public async Task<Result<string>> Handle(UpdateOrganizationLogoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate file
            _fileValidationService.ValidateImageFile(request.LogoFile, "logo");

            // Get organization and validate permissions
            var organization = await _organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);

            if (organization == null)
            {
                return Result<string>.NotFound($"Organization with ID {request.OrganizationId} not found.");
            }

            if (!organization.IsActive)
            {
                return Result<string>.Error("Cannot update logo for inactive organization.");
            }

            // Check if user has permission to update organization logo
            var hasMembership = await _organizationRepository.HasMemberAsync(request.OrganizationId, request.UserId, cancellationToken);
            if (!hasMembership)
            {
                return Result<string>.Forbidden("User does not have permission to update this organization.");
            }

            var oldLogoUrl = organization.LogoUrl;

            // Upload new logo
            string logoUrl;
            using (var stream = request.LogoFile.OpenReadStream())
            {
                logoUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    request.LogoFile.FileName,
                    request.LogoFile.ContentType,
                    "logos");
            }

            // Update organization in database
            organization.UpdateLogo(logoUrl);
            await _organizationRepository.UpdateAsync(organization, cancellationToken);

            // Delete old logo if exists and different
            if (!string.IsNullOrEmpty(oldLogoUrl) && oldLogoUrl != logoUrl)
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(oldLogoUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old logo file {OldLogoUrl}", oldLogoUrl);
                    // Don't fail the operation if we can't delete the old file
                }
            }

            _logger.LogInformation("Successfully updated logo for organization {OrganizationId}", request.OrganizationId);
            return Result.Success(logoUrl);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating logo for organization {OrganizationId}", request.OrganizationId);
            return Result<string>.Error(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating logo for organization {OrganizationId}", request.OrganizationId);
            return Result<string>.Error("Failed to update organization logo: " + ex.Message);
        }
    }
}
