namespace Organizations.Application.Features.Members.Get;

public sealed class GetMemberMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Member, GetMemberResponse>();
    }
}