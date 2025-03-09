namespace Organizations.Infrastructure.Persistence.Repository;

internal class OrganizationRepository(
    ApplicationDbContext context
    ) : BaseRepository<Organization>(context), IOrganizationRepository
{

    public async Task<PaginatedList<Member, T>> GetMembers<T>(Guid organizationId, PaginationOptions paginationOptions)
    {
        return await PaginatedList<Member, T>.CreateAsync(Context.Set<Organization>().Where(o => o.Id == organizationId).SelectMany(o => o.Members), paginationOptions);
    }

    public async Task<PaginatedList<Invitation, T>> GetInvitations<T>(Guid organizationId, PaginationOptions paginationOptions)
    {
        return await PaginatedList<Invitation, T>.CreateAsync(Context.Set<Organization>().Where(o => o.Id == organizationId).SelectMany(o => o.Invitations), paginationOptions);
    }

    public async Task<PaginatedList<Organization, T>> GetOrganizationsByUserIdAsync<T>(Guid userId, PaginationOptions paginationOptions)
    {
        return await PaginatedList<Organization, T>.CreateAsync(Context.Set<Organization>().Where(o => o.Members.Any(m => m.UserId == userId)), paginationOptions);
    }

    public async Task<Organization?> GetByName(string name, CancellationToken cancellationToken)
    {
        return await Context.Set<Organization>().FirstOrDefaultAsync(o => o.Name == name, cancellationToken);
    }

    public async Task<(byte[]? profileImage, byte[]? bannerImage)> GetOrganizationImagesAsync(Guid organizationId)
    {
        var organization = await Context.Set<Organization>()
            .Where(o => o.Id == organizationId)
            .Select(o => new { o.ProfileImage, o.BannerImage })
            .FirstOrDefaultAsync();

        return organization == null
            ? (null, null)
            : (organization.ProfileImage, organization.BannerImage);
    }

    public async Task UpdateOrganizationImagesAsync(Guid organizationId, byte[]? profileImage, byte[]? bannerImage)
    {
        var organization = await Context.Set<Organization>()
            .FirstOrDefaultAsync(o => o.Id == organizationId);

        if (organization == null)
            throw new KeyNotFoundException($"Organization with Id {organizationId} not found.");

        organization.ProfileImage = profileImage;
        organization.BannerImage = bannerImage;

        await Context.SaveChangesAsync();
    }
}