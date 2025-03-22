namespace Organizations.Application.Features.Users.GetUserInvitations;

public sealed class GetUserInvitationsResponse
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    // Add other relevant identity properties you want to expose
}