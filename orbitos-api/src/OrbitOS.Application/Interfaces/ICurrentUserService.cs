namespace OrbitOS.Application.Interfaces;

public interface ICurrentUserService
{
    string? AzureAdObjectId { get; }
    string? Email { get; }
    string? DisplayName { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
}
