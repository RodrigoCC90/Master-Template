using OrbitOS.Domain.Common;

namespace OrbitOS.Domain.Entities;

public class Organization : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? AzureAdTenantId { get; set; }

    // Navigation properties
    public ICollection<OrganizationMembership> Memberships { get; set; } = new List<OrganizationMembership>();
    public ICollection<Function> Functions { get; set; } = new List<Function>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
