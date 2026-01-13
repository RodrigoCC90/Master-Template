namespace OrbitOS.Infrastructure.Data.Seeding;

/// <summary>
/// Deterministic GUIDs for seed data to ensure consistency across migrations.
/// Generated using GUIDs derived from meaningful strings for traceability.
/// </summary>
public static class SeedIds
{
    public static class Organizations
    {
        /// <summary>Rugertek default organization</summary>
        public static readonly Guid Rugertek = new("11111111-1111-1111-1111-111111111111");
    }

    public static class Users
    {
        /// <summary>rodrigo@rugertek.com - Super Admin user</summary>
        public static readonly Guid Rodrigo = new("22222222-2222-2222-2222-222222222222");
    }

    public static class Roles
    {
        /// <summary>Super Administrator role</summary>
        public static readonly Guid SuperAdmin = new("33333333-3333-3333-3333-333333333301");

        /// <summary>User Administrator role</summary>
        public static readonly Guid UserAdmin = new("33333333-3333-3333-3333-333333333302");

        /// <summary>Organization Administrator role</summary>
        public static readonly Guid OrgAdmin = new("33333333-3333-3333-3333-333333333303");

        /// <summary>Viewer role</summary>
        public static readonly Guid Viewer = new("33333333-3333-3333-3333-333333333304");
    }

    public static class Functions
    {
        // User Management Functions
        public static readonly Guid UsersView = new("44444444-4444-4444-4444-444444444401");
        public static readonly Guid UsersCreate = new("44444444-4444-4444-4444-444444444402");
        public static readonly Guid UsersUpdate = new("44444444-4444-4444-4444-444444444403");
        public static readonly Guid UsersDelete = new("44444444-4444-4444-4444-444444444404");
        public static readonly Guid UsersRolesManage = new("44444444-4444-4444-4444-444444444405");
        public static readonly Guid UsersRolesView = new("44444444-4444-4444-4444-444444444406");
        public static readonly Guid UsersInvite = new("44444444-4444-4444-4444-444444444407");
        public static readonly Guid UsersDeactivate = new("44444444-4444-4444-4444-444444444408");
        public static readonly Guid UsersReactivate = new("44444444-4444-4444-4444-444444444409");
        public static readonly Guid UsersActivityView = new("44444444-4444-4444-4444-444444444410");

        // Organization Management Functions
        public static readonly Guid OrgView = new("44444444-4444-4444-4444-444444444501");
        public static readonly Guid OrgUpdate = new("44444444-4444-4444-4444-444444444502");
        public static readonly Guid OrgSettingsManage = new("44444444-4444-4444-4444-444444444503");
        public static readonly Guid OrgSettingsView = new("44444444-4444-4444-4444-444444444504");
        public static readonly Guid OrgBillingManage = new("44444444-4444-4444-4444-444444444505");
        public static readonly Guid OrgBillingView = new("44444444-4444-4444-4444-444444444506");
        public static readonly Guid OrgIntegrationsManage = new("44444444-4444-4444-4444-444444444507");
        public static readonly Guid OrgIntegrationsView = new("44444444-4444-4444-4444-444444444508");
        public static readonly Guid OrgApiKeysManage = new("44444444-4444-4444-4444-444444444509");
        public static readonly Guid OrgApiKeysView = new("44444444-4444-4444-4444-444444444510");
        public static readonly Guid OrgWebhooksManage = new("44444444-4444-4444-4444-444444444511");
        public static readonly Guid OrgWebhooksView = new("44444444-4444-4444-4444-444444444512");
        public static readonly Guid OrgAuditLogView = new("44444444-4444-4444-4444-444444444513");
        public static readonly Guid OrgDataExport = new("44444444-4444-4444-4444-444444444514");

        // Role Management Functions
        public static readonly Guid RolesView = new("44444444-4444-4444-4444-444444444601");
        public static readonly Guid RolesCreate = new("44444444-4444-4444-4444-444444444602");
        public static readonly Guid RolesUpdate = new("44444444-4444-4444-4444-444444444603");
        public static readonly Guid RolesDelete = new("44444444-4444-4444-4444-444444444604");
        public static readonly Guid RolesFunctionsAssign = new("44444444-4444-4444-4444-444444444605");

        // Function Management Functions
        public static readonly Guid FunctionsView = new("44444444-4444-4444-4444-444444444701");
        public static readonly Guid FunctionsCreate = new("44444444-4444-4444-4444-444444444702");
        public static readonly Guid FunctionsUpdate = new("44444444-4444-4444-4444-444444444703");
        public static readonly Guid FunctionsDelete = new("44444444-4444-4444-4444-444444444704");
    }

    public static class Memberships
    {
        /// <summary>Rodrigo's membership in Rugertek</summary>
        public static readonly Guid RodrigoRugertek = new("55555555-5555-5555-5555-555555555501");
    }

    public static class UserRoles
    {
        /// <summary>Rodrigo's SuperAdmin role assignment</summary>
        public static readonly Guid RodrigoSuperAdmin = new("66666666-6666-6666-6666-666666666601");
    }
}
