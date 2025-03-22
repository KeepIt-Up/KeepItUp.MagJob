namespace Organizations.Application.Repository;

public interface IInvitationRepository
{
    Task<Invitation?> GetWithOrganizationByIdAsync(Guid id);
}