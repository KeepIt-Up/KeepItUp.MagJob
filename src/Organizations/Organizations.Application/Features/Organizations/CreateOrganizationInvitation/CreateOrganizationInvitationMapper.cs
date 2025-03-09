namespace Organizations.Application.Features.Organizations.CreateOrganizationInvitation;

public sealed class CreateOrganizationInvitationMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateOrganizationInvitationRequest, CreateOrganizationInvitationResponse>();
    }
}