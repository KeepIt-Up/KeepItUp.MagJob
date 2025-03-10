namespace Organizations.Application.Features.Users.GetUserInvitations;

public sealed class GetUserInvitationsMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, GetUserInvitationsResponse>();
    }
}