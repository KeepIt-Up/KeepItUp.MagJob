namespace KeepItUp.MagJob.Identity.Core.UserAggregate;

/// <summary>
/// Reprezentuje profil użytkownika jako value object.
/// </summary>
public class UserProfile : ValueObject
{
    /// <summary>
    /// Numer telefonu użytkownika.
    /// </summary>
    public string? PhoneNumber { get; }

    /// <summary>
    /// Adres użytkownika.
    /// </summary>
    public string? Address { get; }

    /// <summary>
    /// URL do zdjęcia profilowego użytkownika.
    /// </summary>
    public string? ProfileImage { get; }

    /// <summary>
    /// Tworzy nowy profil użytkownika.
    /// </summary>
    /// <param name="phoneNumber">Numer telefonu użytkownika.</param>
    /// <param name="address">Adres użytkownika.</param>
    /// <param name="profileImage">URL do zdjęcia profilowego użytkownika.</param>
    public UserProfile(string? phoneNumber, string? address, string? profileImage)
    {
        PhoneNumber = phoneNumber;
        Address = address;
        ProfileImage = profileImage;
    }

    /// <summary>
    /// Zwraca komponenty używane do porównywania równości obiektów.
    /// </summary>
    /// <returns>Kolekcja komponentów do porównania.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PhoneNumber ?? string.Empty;
        yield return Address ?? string.Empty;
        yield return ProfileImage ?? string.Empty;
    }

    /// <summary>
    /// Tworzy nowy profil użytkownika z aktualizacją wybranych właściwości.
    /// </summary>
    /// <param name="phoneNumber">Nowy numer telefonu lub null, aby zachować obecny.</param>
    /// <param name="address">Nowy adres lub null, aby zachować obecny.</param>
    /// <param name="profileImage">Nowy URL do zdjęcia profilowego lub null, aby zachować obecny.</param>
    /// <returns>Nowy obiekt UserProfile z zaktualizowanymi właściwościami lub ten sam obiekt, jeśli nic się nie zmieniło.</returns>
    public UserProfile WithUpdates(string? phoneNumber = null, string? address = null, string? profileImage = null)
    {
        var newPhoneNumber = phoneNumber ?? PhoneNumber;
        var newAddress = address ?? Address;
        var newProfileImage = profileImage ?? ProfileImage;

        // Sprawdź czy coś się zmieniło
        if (string.Equals(newPhoneNumber, PhoneNumber) &&
            string.Equals(newAddress, Address) &&
            string.Equals(newProfileImage, ProfileImage))
        {
            return this; // Zwracamy ten sam obiekt jeśli nic się nie zmieniło
        }

        return new UserProfile(newPhoneNumber, newAddress, newProfileImage);
    }
}
