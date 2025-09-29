namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Request for the UpdateUserEndpoint.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// URL template for the UpdateUserEndpoint.
    /// </summary>
    public const string Route = "/Users/{Id:guid}";

    /// <summary>
    /// Builds the URL for the specified user identifier.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>URL with the specified identifier.</returns>
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// User identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Adres użytkownika.
    /// </summary>
    public string? Address { get; set; }
}
