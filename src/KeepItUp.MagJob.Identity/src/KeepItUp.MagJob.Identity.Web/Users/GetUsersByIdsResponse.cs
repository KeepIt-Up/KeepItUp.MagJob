using System.Text.Json.Serialization;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Odpowiedź dla endpointu GetUsersByIds.
/// </summary>
public class GetUsersByIdsResponse
{
    [JsonPropertyName("users")]
    public List<UserDto> Users { get; set; } = new();

    public class UserDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("externalId")]
        public Guid ExternalId { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}

