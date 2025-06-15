using Microsoft.AspNetCore.Http;

namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interface for file validation service
/// </summary>
public interface IFileValidationService
{
    /// <summary>
    /// Validates image file for upload
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <param name="fileType">Type of file being validated (for error messages)</param>
    /// <param name="maxSizeInBytes">Maximum allowed file size in bytes</param>
    void ValidateImageFile(IFormFile file, string fileType, long maxSizeInBytes = 5 * 1024 * 1024);
}