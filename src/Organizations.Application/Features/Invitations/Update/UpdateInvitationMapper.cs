namespace Organizations.Application.Features.Invitations.Update;

public sealed class UpdateInvitationMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Invitation, UpdateInvitationResponse>();
    }
}