namespace Organizations.Application.Features.Invitations.Get;

public sealed class GetInvitationMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Invitation, GetInvitationResponse>()
            .Map(dest => dest.OrganizationId, src => src.Organization.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.OrganizationName, src => src.Organization.Name)
            .Map(dest => dest.UserName, src => "John Doe");
    }
}