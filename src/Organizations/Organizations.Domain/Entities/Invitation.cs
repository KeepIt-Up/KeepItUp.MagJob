namespace Organizations.Domain.Entities;

public class Invitation : BaseEntity
{
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; }
    public Guid UserId { get; set; }
    public InvitationStatus Status { get; set; }

    private Invitation()
    {
        Status = InvitationStatus.Pending;
    }

    public static Invitation Create(Guid organizationId, Guid userId)
    {
        return new Invitation
        {
            OrganizationId = organizationId,
            UserId = userId
        };
    }

    public Member Accept()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException($"Invitation with Id {Id} is not pending.");

        return Organization.AddMember(UserId, "", "");
    }

    public void Reject()
    {
        if (Status != InvitationStatus.Pending)
            throw new InvalidOperationException($"Invitation with Id {Id} is not pending.");

        Status = InvitationStatus.Rejected;
    }

}

public enum InvitationStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled
}