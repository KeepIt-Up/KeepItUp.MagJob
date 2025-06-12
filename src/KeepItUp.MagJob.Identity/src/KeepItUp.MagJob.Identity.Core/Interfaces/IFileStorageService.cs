namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interface for file storage service
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file to the storage service
    /// </summary>
    /// <param name="fileStream">File stream</param>
    /// <param name="fileName">File name</param>
    /// <param name="contentType">Content type (MIME)</param>
    /// <param name="subdirectory">Optional subdirectory</param>
    /// <returns>URL of the uploaded file</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string subdirectory = "");

    /// <summary>
    /// Deletes a file with a specified URL
    /// </summary>
    /// <param name="fileUrl">URL of the file to delete</param>
    Task DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Checks if a file with a specified URL exists
    /// </summary>
    /// <param name="fileUrl">URL of the file to check</param>
    /// <returns>True, if the file exists; otherwise false</returns>
    Task<bool> FileExistsAsync(string fileUrl);
}
