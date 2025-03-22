namespace Organizations.Application.Features.Roles.AddMembers
{
    public class AddMembersToRoleRequest : IRequest<List<Member>>
    {
        public Guid Id { get; set; }
        public List<Guid> MemberIds { get; set; } = new List<Guid>();
    }
}