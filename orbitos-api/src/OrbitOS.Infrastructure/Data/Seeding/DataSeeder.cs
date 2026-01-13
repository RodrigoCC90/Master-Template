using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrbitOS.Application.Interfaces;
using OrbitOS.Domain.Constants;
using OrbitOS.Domain.Entities;
using OrbitOS.Infrastructure.Services;

namespace OrbitOS.Infrastructure.Data.Seeding;

/// <summary>
/// Seeds initial data into the database.
/// Idempotent - checks for existing data before inserting.
/// </summary>
public class DataSeeder : IDataSeeder
{
    private readonly OrbitOSDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(OrbitOSDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting data seeding...");

        await SeedOrganizationsAsync(cancellationToken);
        await SeedUsersAsync(cancellationToken);
        await SeedOrganizationMembershipsAsync(cancellationToken);
        await SeedFunctionsAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedRoleFunctionsAsync(cancellationToken);
        await SeedUserRolesAsync(cancellationToken);

        _logger.LogInformation("Data seeding completed successfully");
    }

    private async Task SeedOrganizationsAsync(CancellationToken cancellationToken)
    {
        if (await _context.Organizations.AnyAsync(o => o.Id == SeedIds.Organizations.Rugertek, cancellationToken))
        {
            _logger.LogDebug("Organizations already seeded");
            return;
        }

        var organization = new Organization
        {
            Id = SeedIds.Organizations.Rugertek,
            Name = "Rugertek",
            Slug = "rugertek",
            Description = "Rugertek - AI-Native Business Solutions",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded organization: {Organization}", organization.Name);
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Id == SeedIds.Users.Rodrigo, cancellationToken))
        {
            _logger.LogDebug("Users already seeded");
            return;
        }

        var user = new User
        {
            Id = SeedIds.Users.Rodrigo,
            Email = "rodrigo@rugertek.com",
            DisplayName = "Rodrigo Campos Cervera",
            FirstName = "Rodrigo",
            LastName = "Campos Cervera",
            PasswordHash = PasswordHasher.HashPassword("123456"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded user: {Email}", user.Email);
    }

    private async Task SeedOrganizationMembershipsAsync(CancellationToken cancellationToken)
    {
        if (await _context.OrganizationMemberships.AnyAsync(m => m.Id == SeedIds.Memberships.RodrigoRugertek, cancellationToken))
        {
            _logger.LogDebug("Organization memberships already seeded");
            return;
        }

        var membership = new OrganizationMembership
        {
            Id = SeedIds.Memberships.RodrigoRugertek,
            UserId = SeedIds.Users.Rodrigo,
            OrganizationId = SeedIds.Organizations.Rugertek,
            Role = MembershipRole.Owner,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.OrganizationMemberships.Add(membership);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded membership for user {UserId} in organization {OrgId}", membership.UserId, membership.OrganizationId);
    }

    private async Task SeedFunctionsAsync(CancellationToken cancellationToken)
    {
        if (await _context.Functions.AnyAsync(f => f.Id == SeedIds.Functions.UsersView, cancellationToken))
        {
            _logger.LogDebug("Functions already seeded");
            return;
        }

        var functions = GetSystemFunctions();

        _context.Functions.AddRange(functions);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} system functions", functions.Count);
    }

    private List<Function> GetSystemFunctions()
    {
        var now = DateTime.UtcNow;
        var orgId = SeedIds.Organizations.Rugertek;

        return new List<Function>
        {
            // User Management Functions
            new() { Id = SeedIds.Functions.UsersView, Name = SystemFunctions.UserManagement.ViewUsers, Description = "View user profiles and information", Purpose = "Read access to user data", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersCreate, Name = SystemFunctions.UserManagement.CreateUsers, Description = "Create new user accounts", Purpose = "Add new users to the organization", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersUpdate, Name = SystemFunctions.UserManagement.UpdateUsers, Description = "Update user profile information", Purpose = "Modify existing user data", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersDelete, Name = SystemFunctions.UserManagement.DeleteUsers, Description = "Delete user accounts", Purpose = "Remove users from the organization", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersRolesManage, Name = SystemFunctions.UserManagement.ManageUserRoles, Description = "Assign and remove roles from users", Purpose = "Control user permissions", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersRolesView, Name = SystemFunctions.UserManagement.ViewUserRoles, Description = "View role assignments for users", Purpose = "See what roles users have", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersInvite, Name = SystemFunctions.UserManagement.InviteUsers, Description = "Send invitations to new users", Purpose = "Onboard new team members", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersDeactivate, Name = SystemFunctions.UserManagement.DeactivateUsers, Description = "Deactivate user accounts", Purpose = "Temporarily disable user access", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersReactivate, Name = SystemFunctions.UserManagement.ReactivateUsers, Description = "Reactivate deactivated user accounts", Purpose = "Restore user access", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.UsersActivityView, Name = SystemFunctions.UserManagement.ViewUserActivity, Description = "View user activity logs", Purpose = "Monitor user actions", Category = SystemFunctions.UserManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },

            // Organization Management Functions
            new() { Id = SeedIds.Functions.OrgView, Name = SystemFunctions.OrganizationManagement.ViewOrganization, Description = "View organization information", Purpose = "Read access to organization data", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgUpdate, Name = SystemFunctions.OrganizationManagement.UpdateOrganization, Description = "Update organization information", Purpose = "Modify organization profile", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgSettingsManage, Name = SystemFunctions.OrganizationManagement.ManageSettings, Description = "Manage organization settings", Purpose = "Configure organization preferences", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgSettingsView, Name = SystemFunctions.OrganizationManagement.ViewSettings, Description = "View organization settings", Purpose = "Read organization configuration", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgBillingManage, Name = SystemFunctions.OrganizationManagement.ManageBilling, Description = "Manage billing and subscriptions", Purpose = "Handle payments and plans", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgBillingView, Name = SystemFunctions.OrganizationManagement.ViewBilling, Description = "View billing information", Purpose = "See payment history and invoices", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgIntegrationsManage, Name = SystemFunctions.OrganizationManagement.ManageIntegrations, Description = "Manage third-party integrations", Purpose = "Configure external services", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgIntegrationsView, Name = SystemFunctions.OrganizationManagement.ViewIntegrations, Description = "View integration configurations", Purpose = "See connected services", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgApiKeysManage, Name = SystemFunctions.OrganizationManagement.ManageApiKeys, Description = "Create and revoke API keys", Purpose = "Manage programmatic access", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgApiKeysView, Name = SystemFunctions.OrganizationManagement.ViewApiKeys, Description = "View API keys", Purpose = "See existing API keys", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgWebhooksManage, Name = SystemFunctions.OrganizationManagement.ManageWebhooks, Description = "Configure webhook endpoints", Purpose = "Set up event notifications", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgWebhooksView, Name = SystemFunctions.OrganizationManagement.ViewWebhooks, Description = "View webhook configurations", Purpose = "See webhook settings", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgAuditLogView, Name = SystemFunctions.OrganizationManagement.ViewAuditLog, Description = "View organization audit log", Purpose = "Review security and compliance events", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.OrgDataExport, Name = SystemFunctions.OrganizationManagement.ExportData, Description = "Export organization data", Purpose = "Download data for backup or migration", Category = SystemFunctions.OrganizationManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },

            // Role Management Functions
            new() { Id = SeedIds.Functions.RolesView, Name = SystemFunctions.RoleManagement.ViewRoles, Description = "View roles", Purpose = "See available roles", Category = SystemFunctions.RoleManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.RolesCreate, Name = SystemFunctions.RoleManagement.CreateRoles, Description = "Create new roles", Purpose = "Define custom permission sets", Category = SystemFunctions.RoleManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.RolesUpdate, Name = SystemFunctions.RoleManagement.UpdateRoles, Description = "Update existing roles", Purpose = "Modify role definitions", Category = SystemFunctions.RoleManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.RolesDelete, Name = SystemFunctions.RoleManagement.DeleteRoles, Description = "Delete roles", Purpose = "Remove unused roles", Category = SystemFunctions.RoleManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.RolesFunctionsAssign, Name = SystemFunctions.RoleManagement.AssignFunctions, Description = "Assign functions to roles", Purpose = "Configure role permissions", Category = SystemFunctions.RoleManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },

            // Function Management Functions
            new() { Id = SeedIds.Functions.FunctionsView, Name = SystemFunctions.FunctionManagement.ViewFunctions, Description = "View functions", Purpose = "See available permissions", Category = SystemFunctions.FunctionManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.FunctionsCreate, Name = SystemFunctions.FunctionManagement.CreateFunctions, Description = "Create custom functions", Purpose = "Define new permissions", Category = SystemFunctions.FunctionManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.FunctionsUpdate, Name = SystemFunctions.FunctionManagement.UpdateFunctions, Description = "Update functions", Purpose = "Modify permission definitions", Category = SystemFunctions.FunctionManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
            new() { Id = SeedIds.Functions.FunctionsDelete, Name = SystemFunctions.FunctionManagement.DeleteFunctions, Description = "Delete functions", Purpose = "Remove unused permissions", Category = SystemFunctions.FunctionManagement.Category, OrganizationId = orgId, CreatedAt = now, UpdatedAt = now },
        };
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        if (await _context.Roles.AnyAsync(r => r.Id == SeedIds.Roles.SuperAdmin, cancellationToken))
        {
            _logger.LogDebug("Roles already seeded");
            return;
        }

        var now = DateTime.UtcNow;
        var orgId = SeedIds.Organizations.Rugertek;

        var roles = new List<Role>
        {
            new()
            {
                Id = SeedIds.Roles.SuperAdmin,
                Name = SystemRoles.SuperAdmin,
                Description = "Full access to all system features including user and organization management",
                Purpose = "Ultimate administrative control",
                Department = "Administration",
                OrganizationId = orgId,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Id = SeedIds.Roles.UserAdmin,
                Name = SystemRoles.UserAdmin,
                Description = "Full access to user management features",
                Purpose = "Manage users and their permissions",
                Department = "Human Resources",
                OrganizationId = orgId,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Id = SeedIds.Roles.OrgAdmin,
                Name = SystemRoles.OrgAdmin,
                Description = "Full access to organization management features",
                Purpose = "Configure organization settings and integrations",
                Department = "Operations",
                OrganizationId = orgId,
                CreatedAt = now,
                UpdatedAt = now
            },
            new()
            {
                Id = SeedIds.Roles.Viewer,
                Name = SystemRoles.Viewer,
                Description = "Read-only access to view users and organization information",
                Purpose = "Observation without modification capabilities",
                Department = "General",
                OrganizationId = orgId,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        _context.Roles.AddRange(roles);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} system roles", roles.Count);
    }

    private async Task SeedRoleFunctionsAsync(CancellationToken cancellationToken)
    {
        if (await _context.RoleFunctions.AnyAsync(cancellationToken))
        {
            _logger.LogDebug("Role-function assignments already seeded");
            return;
        }

        var now = DateTime.UtcNow;
        var roleFunctions = new List<RoleFunction>();

        // Super Admin gets ALL functions
        var allFunctionIds = GetAllFunctionIds();
        foreach (var functionId in allFunctionIds)
        {
            roleFunctions.Add(new RoleFunction
            {
                Id = Guid.NewGuid(),
                RoleId = SeedIds.Roles.SuperAdmin,
                FunctionId = functionId,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        // User Admin gets all User Management + Role Management + Function View
        var userAdminFunctionIds = new[]
        {
            // All User Management
            SeedIds.Functions.UsersView, SeedIds.Functions.UsersCreate, SeedIds.Functions.UsersUpdate,
            SeedIds.Functions.UsersDelete, SeedIds.Functions.UsersRolesManage, SeedIds.Functions.UsersRolesView,
            SeedIds.Functions.UsersInvite, SeedIds.Functions.UsersDeactivate, SeedIds.Functions.UsersReactivate,
            SeedIds.Functions.UsersActivityView,
            // All Role Management
            SeedIds.Functions.RolesView, SeedIds.Functions.RolesCreate, SeedIds.Functions.RolesUpdate,
            SeedIds.Functions.RolesDelete, SeedIds.Functions.RolesFunctionsAssign,
            // Functions view only
            SeedIds.Functions.FunctionsView
        };

        foreach (var functionId in userAdminFunctionIds)
        {
            roleFunctions.Add(new RoleFunction
            {
                Id = Guid.NewGuid(),
                RoleId = SeedIds.Roles.UserAdmin,
                FunctionId = functionId,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        // Org Admin gets all Organization Management + view functions
        var orgAdminFunctionIds = new[]
        {
            // All Organization Management
            SeedIds.Functions.OrgView, SeedIds.Functions.OrgUpdate, SeedIds.Functions.OrgSettingsManage,
            SeedIds.Functions.OrgSettingsView, SeedIds.Functions.OrgBillingManage, SeedIds.Functions.OrgBillingView,
            SeedIds.Functions.OrgIntegrationsManage, SeedIds.Functions.OrgIntegrationsView,
            SeedIds.Functions.OrgApiKeysManage, SeedIds.Functions.OrgApiKeysView,
            SeedIds.Functions.OrgWebhooksManage, SeedIds.Functions.OrgWebhooksView,
            SeedIds.Functions.OrgAuditLogView, SeedIds.Functions.OrgDataExport,
            // View users and roles
            SeedIds.Functions.UsersView, SeedIds.Functions.UsersRolesView,
            SeedIds.Functions.RolesView, SeedIds.Functions.FunctionsView
        };

        foreach (var functionId in orgAdminFunctionIds)
        {
            roleFunctions.Add(new RoleFunction
            {
                Id = Guid.NewGuid(),
                RoleId = SeedIds.Roles.OrgAdmin,
                FunctionId = functionId,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        // Viewer gets only view permissions
        var viewerFunctionIds = new[]
        {
            SeedIds.Functions.UsersView, SeedIds.Functions.UsersRolesView,
            SeedIds.Functions.OrgView, SeedIds.Functions.OrgSettingsView,
            SeedIds.Functions.OrgBillingView, SeedIds.Functions.OrgIntegrationsView,
            SeedIds.Functions.OrgApiKeysView, SeedIds.Functions.OrgWebhooksView,
            SeedIds.Functions.RolesView, SeedIds.Functions.FunctionsView
        };

        foreach (var functionId in viewerFunctionIds)
        {
            roleFunctions.Add(new RoleFunction
            {
                Id = Guid.NewGuid(),
                RoleId = SeedIds.Roles.Viewer,
                FunctionId = functionId,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        _context.RoleFunctions.AddRange(roleFunctions);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} role-function assignments", roleFunctions.Count);
    }

    private static Guid[] GetAllFunctionIds()
    {
        return new[]
        {
            // User Management
            SeedIds.Functions.UsersView, SeedIds.Functions.UsersCreate, SeedIds.Functions.UsersUpdate,
            SeedIds.Functions.UsersDelete, SeedIds.Functions.UsersRolesManage, SeedIds.Functions.UsersRolesView,
            SeedIds.Functions.UsersInvite, SeedIds.Functions.UsersDeactivate, SeedIds.Functions.UsersReactivate,
            SeedIds.Functions.UsersActivityView,
            // Organization Management
            SeedIds.Functions.OrgView, SeedIds.Functions.OrgUpdate, SeedIds.Functions.OrgSettingsManage,
            SeedIds.Functions.OrgSettingsView, SeedIds.Functions.OrgBillingManage, SeedIds.Functions.OrgBillingView,
            SeedIds.Functions.OrgIntegrationsManage, SeedIds.Functions.OrgIntegrationsView,
            SeedIds.Functions.OrgApiKeysManage, SeedIds.Functions.OrgApiKeysView,
            SeedIds.Functions.OrgWebhooksManage, SeedIds.Functions.OrgWebhooksView,
            SeedIds.Functions.OrgAuditLogView, SeedIds.Functions.OrgDataExport,
            // Role Management
            SeedIds.Functions.RolesView, SeedIds.Functions.RolesCreate, SeedIds.Functions.RolesUpdate,
            SeedIds.Functions.RolesDelete, SeedIds.Functions.RolesFunctionsAssign,
            // Function Management
            SeedIds.Functions.FunctionsView, SeedIds.Functions.FunctionsCreate, SeedIds.Functions.FunctionsUpdate,
            SeedIds.Functions.FunctionsDelete
        };
    }

    private async Task SeedUserRolesAsync(CancellationToken cancellationToken)
    {
        if (await _context.UserRoles.AnyAsync(ur => ur.Id == SeedIds.UserRoles.RodrigoSuperAdmin, cancellationToken))
        {
            _logger.LogDebug("User roles already seeded");
            return;
        }

        var userRole = new UserRole
        {
            Id = SeedIds.UserRoles.RodrigoSuperAdmin,
            UserId = SeedIds.Users.Rodrigo,
            RoleId = SeedIds.Roles.SuperAdmin,
            OrganizationId = SeedIds.Organizations.Rugertek,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded Super Admin role for user: rodrigo@rugertek.com");
    }
}
