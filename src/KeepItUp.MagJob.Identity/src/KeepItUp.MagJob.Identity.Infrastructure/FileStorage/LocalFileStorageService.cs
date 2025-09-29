using System.Text.RegularExpressions;
using KeepItUp.MagJob.Identity.Core.Interfaces;

namespace KeepItUp.MagJob.Identity.Infrastructure.FileStorage;

/// <summary>
/// Implementation of the local file storage service
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;
    private readonly ILogger<LocalFileStorageService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileStorageService"/> class.
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <param name="logger">Logger</param>
    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _basePath = configuration["FileStorage:BasePath"] ?? "./wwwroot/uploads";
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "http://localhost:5000/uploads";
        _logger = logger;

        // Ensure base directory exists
        EnsureBaseDirectoryExists();
    }

    private void EnsureBaseDirectoryExists()
    {
        try
        {
            if (!Directory.Exists(_basePath))
            {
                _logger.LogInformation("Creating base directory: {BasePath}", _basePath);
                Directory.CreateDirectory(_basePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create base directory: {BasePath}", _basePath);
        }
    }

    /// <inheritdoc />
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string subdirectory = "")
    {
        // Sanitize the file name to prevent security issues
        string safeFileName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "-");
        safeFileName = Regex.Replace(safeFileName, @"[^\w\-]", "");

        string extension = Path.GetExtension(fileName);
        string uniqueFileName = $"{safeFileName}-{Guid.NewGuid()}{extension}";

        // Create subdirectory path
        string directoryPath = _basePath;
        if (!string.IsNullOrEmpty(subdirectory))
        {
            directoryPath = Path.Combine(_basePath, subdirectory);
        }

        // Ensure directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, uniqueFileName);

        using (var fileStream2 = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStream2);
        }

        string fileUrl = _baseUrl;
        if (!string.IsNullOrEmpty(subdirectory))
        {
            fileUrl = $"{_baseUrl}/{subdirectory}";
        }

        return $"{fileUrl}/{uniqueFileName}";
    }

    /// <inheritdoc />
    public Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return Task.CompletedTask;

        // Extract file path from URL
        if (!fileUrl.StartsWith(_baseUrl))
        {
            _logger.LogWarning("Attempted to delete file with URL that doesn't match base URL: {FileUrl}", fileUrl);
            return Task.CompletedTask;
        }

        string relativePath = fileUrl.Substring(_baseUrl.Length).TrimStart('/');
        string fullPath = Path.Combine(_basePath, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Deleted file: {FilePath}", fullPath);
        }
        else
        {
            _logger.LogWarning("File not found for deletion: {FilePath}", fullPath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> FileExistsAsync(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl))
            return Task.FromResult(false);

        // Extract file path from URL
        if (!fileUrl.StartsWith(_baseUrl))
        {
            return Task.FromResult(false);
        }

        string relativePath = fileUrl.Substring(_baseUrl.Length).TrimStart('/');
        string fullPath = Path.Combine(_basePath, relativePath);

        return Task.FromResult(File.Exists(fullPath));
    }
}
