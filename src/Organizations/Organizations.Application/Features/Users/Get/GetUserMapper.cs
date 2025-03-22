namespace Organizations.Application.Features.Users.Get;

public sealed class GetUserMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, GetUserResponse>();
    }
}