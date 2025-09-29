using KeepItUp.MagJob.Identity.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace KeepItUp.MagJob.Identity.Infrastructure.Services;

/// <summary>
/// Service for validating uploaded files
/// </summary>
public class FileValidationService : IFileValidationService
{
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };

    /// <inheritdoc />
    public void ValidateImageFile(IFormFile file, string fileType, long maxSizeInBytes = 5 * 1024 * 1024)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException($"{fileType} file is required.");
        }

        if (file.Length > maxSizeInBytes)
        {
            throw new ArgumentException($"{fileType} file size cannot exceed {maxSizeInBytes / (1024 * 1024)}MB.");
        }

        if (!AllowedImageTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            throw new ArgumentException($"Only {string.Join(", ", AllowedImageTypes)} files are allowed for {fileType}.");
        }
    }
}