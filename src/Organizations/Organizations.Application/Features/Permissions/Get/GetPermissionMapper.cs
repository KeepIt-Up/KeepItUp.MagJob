namespace Organizations.Application.Features.Permissions.Get;

public sealed class GetPermissionMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Permission, GetPermissionResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description);
    }
}