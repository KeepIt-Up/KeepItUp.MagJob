namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Request for the GetUserEndpoint.
/// </summary>
public class GetUserByIdRequest
{
    /// <summary>
    /// URL template for the GetUserEndpoint.
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
}
