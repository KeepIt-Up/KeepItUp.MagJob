namespace Organizations.Application.Features.Roles.RemoveMembers
{
    public class RemoveMembersFromRoleRequest
    {
        public Guid Id { get; set; }
        public List<Guid> MemberIds { get; set; } = new List<Guid>();
    }
}