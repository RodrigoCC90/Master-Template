namespace OrbitOS.Domain.Constants;

/// <summary>
/// System-level functions representing granular permissions.
/// These are organized by category (User Management, Organization Management, etc.)
/// </summary>
public static class SystemFunctions
{
    public static class UserManagement
    {
        public const string Category = "User Management";

        public const string ViewUsers = "users.view";
        public const string CreateUsers = "users.create";
        public const string UpdateUsers = "users.update";
        public const string DeleteUsers = "users.delete";
        public const string ManageUserRoles = "users.roles.manage";
        public const string ViewUserRoles = "users.roles.view";
        public const string InviteUsers = "users.invite";
        public const string DeactivateUsers = "users.deactivate";
        public const string ReactivateUsers = "users.reactivate";
        public const string ViewUserActivity = "users.activity.view";

        public static readonly string[] All =
        {
            ViewUsers, CreateUsers, UpdateUsers, DeleteUsers,
            ManageUserRoles, ViewUserRoles, InviteUsers,
            DeactivateUsers, ReactivateUsers, ViewUserActivity
        };
    }

    public static class OrganizationManagement
    {
        public const string Category = "Organization Management";

        public const string ViewOrganization = "organization.view";
        public const string UpdateOrganization = "organization.update";
        public const string ManageSettings = "organization.settings.manage";
        public const string ViewSettings = "organization.settings.view";
        public const string ManageBilling = "organization.billing.manage";
        public const string ViewBilling = "organization.billing.view";
        public const string ManageIntegrations = "organization.integrations.manage";
        public const string ViewIntegrations = "organization.integrations.view";
        public const string ManageApiKeys = "organization.apikeys.manage";
        public const string ViewApiKeys = "organization.apikeys.view";
        public const string ManageWebhooks = "organization.webhooks.manage";
        public const string ViewWebhooks = "organization.webhooks.view";
        public const string ViewAuditLog = "organization.auditlog.view";
        public const string ExportData = "organization.data.export";

        public static readonly string[] All =
        {
            ViewOrganization, UpdateOrganization,
            ManageSettings, ViewSettings,
            ManageBilling, ViewBilling,
            ManageIntegrations, ViewIntegrations,
            ManageApiKeys, ViewApiKeys,
            ManageWebhooks, ViewWebhooks,
            ViewAuditLog, ExportData
        };
    }

    public static class RoleManagement
    {
        public const string Category = "Role Management";

        public const string ViewRoles = "roles.view";
        public const string CreateRoles = "roles.create";
        public const string UpdateRoles = "roles.update";
        public const string DeleteRoles = "roles.delete";
        public const string AssignFunctions = "roles.functions.assign";

        public static readonly string[] All =
        {
            ViewRoles, CreateRoles, UpdateRoles, DeleteRoles, AssignFunctions
        };
    }

    public static class FunctionManagement
    {
        public const string Category = "Function Management";

        public const string ViewFunctions = "functions.view";
        public const string CreateFunctions = "functions.create";
        public const string UpdateFunctions = "functions.update";
        public const string DeleteFunctions = "functions.delete";

        public static readonly string[] All =
        {
            ViewFunctions, CreateFunctions, UpdateFunctions, DeleteFunctions
        };
    }
}
