namespace Organizations.Application.Features.Organizations.Create;

public sealed class CreateOrganizationMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateOrganizationRequest, CreateOrganizationResponse>();
    }
}