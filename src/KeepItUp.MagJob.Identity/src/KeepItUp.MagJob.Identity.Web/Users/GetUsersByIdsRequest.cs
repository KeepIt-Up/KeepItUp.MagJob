using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Request do pobierania wielu użytkowników.
/// </summary>
public class GetUsersByIdsRequest
{
    public const string Route = "/users/batch";
    
    [JsonPropertyName("ids")]
    public List<Guid> Ids { get; set; } = new();
}

