using System.IO;
using System.Threading.Tasks;

namespace KeepItUp.MagJob.Identity.Core.Interfaces;

/// <summary>
/// Interfejs serwisu do zarządzania plikami
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Przesyła plik do serwisu przechowywania
    /// </summary>
    /// <param name="fileStream">Strumień pliku</param>
    /// <param name="fileName">Nazwa pliku</param>
    /// <param name="contentType">Typ zawartości (MIME)</param>
    /// <param name="subdirectory">Opcjonalny podkatalog</param>
    /// <returns>URL do przesłanego pliku</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string subdirectory = "");

    /// <summary>
    /// Usuwa plik o określonym URL
    /// </summary>
    /// <param name="fileUrl">URL pliku do usunięcia</param>
    Task DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Sprawdza, czy plik o określonym URL istnieje
    /// </summary>
    /// <param name="fileUrl">URL pliku do sprawdzenia</param>
    /// <returns>True, jeśli plik istnieje; w przeciwnym razie false</returns>
    Task<bool> FileExistsAsync(string fileUrl);
}
