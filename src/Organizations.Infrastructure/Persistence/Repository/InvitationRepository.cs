namespace Organizations.Infrastructure.Persistence.Repository;

internal class InvitationRepository(
    ApplicationDbContext context
    ) : BaseRepository<Invitation>(context), IInvitationRepository
{
    public async Task<Invitation?> GetWithOrganizationByIdAsync(Guid id)
    {
        return await Context.Invitations.Include(i => i.Organization).FirstOrDefaultAsync(i => i.Id == id);
    }
}
