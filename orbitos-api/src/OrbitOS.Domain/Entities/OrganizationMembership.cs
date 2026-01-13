using OrbitOS.Domain.Common;

namespace OrbitOS.Domain.Entities;

public class OrganizationMembership : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public MembershipRole Role { get; set; } = MembershipRole.Member;

    // Navigation properties
    public User User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}

public enum MembershipRole
{
    Member = 0,
    Admin = 1,
    Owner = 2
}
