namespace Organizations.Application.Features.Users.Get;

public sealed class GetUserResponse
{
    public Guid Id { get; init; }
    public string Firstname { get; init; } = string.Empty;
    public string Lastname { get; init; } = string.Empty;
}