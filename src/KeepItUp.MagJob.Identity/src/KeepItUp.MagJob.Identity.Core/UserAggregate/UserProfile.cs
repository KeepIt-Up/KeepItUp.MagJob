namespace KeepItUp.MagJob.Identity.Core.UserAggregate;

/// <summary>
/// Represents the user's profile as a value object.
/// </summary>
public class UserProfile : ValueObject
{
    /// <summary>
    /// User's phone number.
    /// </summary>
    public string? PhoneNumber { get; }

    /// <summary>
    /// User's address.
    /// </summary>
    public string? Address { get; }

    /// <summary>
    /// URL of the user's profile picture.
    /// </summary>
    public string? ProfileImage { get; }

    /// <summary>
    /// Creates a new user profile.
    /// </summary>
    /// <param name="phoneNumber">User's phone number.</param>
    /// <param name="address">User's address.</param>
    /// <param name="profileImage">URL of the user's profile picture.</param>
    public UserProfile(string? phoneNumber, string? address, string? profileImage)
    {
        PhoneNumber = phoneNumber;
        Address = address;
        ProfileImage = profileImage;
    }

    /// <summary>
    /// Returns the components used to compare object equality.
    /// </summary>
    /// <returns>Collection of components to compare.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PhoneNumber ?? string.Empty;
        yield return Address ?? string.Empty;
        yield return ProfileImage ?? string.Empty;
    }

    /// <summary>
    /// Creates a new user profile with updates to selected properties.
    /// </summary>
    /// <param name="phoneNumber">New phone number or null to keep the current one.</param>
    /// <param name="address">New address or null, to keep the current one.</param>
    /// <param name="profileImage">New profile picture URL or null, to keep the current one.</param>
    /// <returns>New UserProfile object with updated properties or the same object, if nothing changed.</returns>
    public UserProfile WithUpdates(string? phoneNumber = null, string? address = null, string? profileImage = null)
    {
        var newPhoneNumber = phoneNumber ?? PhoneNumber;
        var newAddress = address ?? Address;
        var newProfileImage = profileImage ?? ProfileImage;

        // Check if something changed
        if (string.Equals(newPhoneNumber, PhoneNumber) &&
            string.Equals(newAddress, Address) &&
            string.Equals(newProfileImage, ProfileImage))
        {
            return this; // Return the same object if nothing changed
        }

        return new UserProfile(newPhoneNumber, newAddress, newProfileImage);
    }
}
