using OrbitOS.Domain.Common;

namespace OrbitOS.Domain.Entities;

public class RoleFunction : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid FunctionId { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Function Function { get; set; } = null!;
}
