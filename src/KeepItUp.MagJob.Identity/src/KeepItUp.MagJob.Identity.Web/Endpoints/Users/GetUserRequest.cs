namespace KeepItUp.MagJob.Identity.Web.Endpoints.Users;

/// <summary>
/// Żądanie dla endpointu GetUserEndpoint.
/// </summary>
public class GetUserRequest
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }
} 
