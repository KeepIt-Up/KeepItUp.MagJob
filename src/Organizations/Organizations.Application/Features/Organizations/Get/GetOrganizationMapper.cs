namespace Organizations.Application.Features.Organizations.Get;

public sealed class GetOrganizationMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Organization, GetOrganizationResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.OwnerId, src => src.OwnerId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.ProfileImage, src => src.ProfileImage)
            .Map(dest => dest.BannerImage, src => src.BannerImage);
    }
}