namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Żądanie dla endpointu GetUserEndpoint.
/// </summary>
public class GetUserByIdRequest
{
    public const string Route = "/Users/{Id:guid}";
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }
} 
