namespace OrbitOS.Application.Interfaces;

/// <summary>
/// Interface for seeding initial/demo data into the database.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Seeds all initial data if not already present.
    /// Idempotent - safe to call multiple times.
    /// </summary>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
