namespace Organizations.Application.Features.Roles.Get;

public sealed class GetRoleMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Role, GetRoleResponse>();
    }
}