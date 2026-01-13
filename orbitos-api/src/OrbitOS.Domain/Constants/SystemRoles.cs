namespace OrbitOS.Domain.Constants;

/// <summary>
/// System-level roles with predefined permissions.
/// These roles are seeded into every organization.
/// </summary>
public static class SystemRoles
{
    /// <summary>
    /// Super Administrator with full access to all functions.
    /// Typically assigned to organization owners.
    /// </summary>
    public const string SuperAdmin = "Super Administrator";

    /// <summary>
    /// User Administrator with full user management capabilities.
    /// Can manage users, roles, and assignments but not organization settings.
    /// </summary>
    public const string UserAdmin = "User Administrator";

    /// <summary>
    /// Organization Administrator with full organization management capabilities.
    /// Can manage organization settings, integrations, and billing.
    /// </summary>
    public const string OrgAdmin = "Organization Administrator";

    /// <summary>
    /// Read-only viewer with view access to users and organization info.
    /// Cannot make any changes.
    /// </summary>
    public const string Viewer = "Viewer";

    public static readonly string[] All =
    {
        SuperAdmin, UserAdmin, OrgAdmin, Viewer
    };
}
