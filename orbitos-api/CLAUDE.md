# OrbitOS API - AI Development Guide

## Overview

This is the **.NET 8 Backend API** for OrbitOS. It implements a Clean Architecture pattern with CQRS using MediatR.

## Architecture

```
orbitos-api/
├── src/
│   ├── OrbitOS.Api/              # Web API layer (Controllers, Middleware)
│   ├── OrbitOS.Application/      # Use cases (Commands, Queries, Handlers)
│   ├── OrbitOS.Domain/           # Core domain (Entities, Rules, Events)
│   ├── OrbitOS.Infrastructure/   # External concerns (EF Core, Services)
│   └── OrbitOS.Contracts/        # DTOs and API models
├── tests/
│   ├── OrbitOS.UnitTests/        # Domain and Application tests
│   ├── OrbitOS.IntegrationTests/ # API and Database tests
│   └── OrbitOS.ArchTests/        # Architecture enforcement
└── .ai/                          # AI context files
```

## Layer Dependencies

```
Api → Application → Domain
 ↓         ↓
Infrastructure (implements interfaces from Domain)
```

**Rules:**
- Domain has NO external dependencies (pure C#)
- Application depends only on Domain
- Infrastructure implements interfaces defined in Domain
- Api orchestrates everything via DI

## Tech Stack

| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | .NET | 8.0 LTS |
| ORM | Entity Framework Core | 8.x |
| CQRS | MediatR | 12.x |
| Validation | FluentValidation | 11.x |
| Mapping | Mapster | 7.x |
| Auth | JWT Bearer + OIDC | - |
| Testing | xUnit + FluentAssertions | - |
| Containers | TestContainers | 3.x |

## Key Patterns

### 1. Entity Pattern (Domain Layer)

All entities inherit from `BaseEntity` with audit fields:

```csharp
// src/OrbitOS.Domain/Common/BaseEntity.cs
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }  // Soft delete
}
```

### 2. Organization-Scoped Entity

All org-scoped entities MUST include `OrganizationId`:

```csharp
// src/OrbitOS.Domain/Entities/Canvas.cs
public class Canvas : BaseEntity
{
    public Guid OrganizationId { get; private set; }  // REQUIRED
    public string Name { get; private set; }
    public CanvasType Type { get; private set; }
    public CanvasState State { get; private set; }
    // ...
}
```

### 3. CQRS Command/Query Pattern

```csharp
// Command (Application layer)
public record CreateCanvasCommand(
    Guid OrganizationId,
    string Name,
    CanvasType Type
) : IRequest<Result<CanvasDto>>;

// Handler
public class CreateCanvasHandler : IRequestHandler<CreateCanvasCommand, Result<CanvasDto>>
{
    public async Task<Result<CanvasDto>> Handle(CreateCanvasCommand request, CancellationToken ct)
    {
        // 1. Validate business rules
        // 2. Create entity
        // 3. Persist
        // 4. Return result
    }
}
```

### 4. Repository Pattern with Multi-Tenancy

```csharp
// Always scope by organization
public interface ICanvasRepository
{
    Task<Canvas?> GetByIdAsync(Guid orgId, Guid canvasId, CancellationToken ct);
    Task<IReadOnlyList<Canvas>> GetAllAsync(Guid orgId, CancellationToken ct);
    // Organization ID is ALWAYS first parameter
}
```

### 5. Global Query Filters (EF Core)

```csharp
// Automatic multi-tenancy and soft-delete filtering
protected override void OnModelCreating(ModelBuilder builder)
{
    // Soft delete filter
    builder.Entity<Canvas>().HasQueryFilter(c => c.DeletedAt == null);

    // Multi-tenant filter (set via ITenantContext)
    builder.Entity<Canvas>().HasQueryFilter(c => c.OrganizationId == _tenantId);
}
```

## Implementing an Entity from Specs

When implementing an entity (e.g., ENT-005 Canvas):

### Step 1: Read the Spec
```bash
cat ../specs/L4-data/entities/ENT-005-canvas.json
```

### Step 2: Create Domain Entity
```csharp
// src/OrbitOS.Domain/Entities/Canvas.cs
namespace OrbitOS.Domain.Entities;

public class Canvas : BaseEntity, IAggregateRoot
{
    // Map fields from spec
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public CanvasType Type { get; private set; }
    public CanvasState State { get; private set; }
    public int Version { get; private set; }
    public JsonDocument? BlocksData { get; private set; }

    // Navigation properties
    public Organization Organization { get; private set; } = null!;

    // Private constructor for EF
    private Canvas() { }

    // Factory method with validation
    public static Result<Canvas> Create(
        Guid organizationId,
        string name,
        CanvasType type,
        Guid createdBy)
    {
        // Validate per spec rules
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Canvas>("Name is required");

        return Result.Success(new Canvas
        {
            OrganizationId = organizationId,
            Name = name,
            Type = type,
            State = CanvasState.Draft,
            Version = 1,
            CreatedBy = createdBy
        });
    }
}
```

### Step 3: Create EF Configuration
```csharp
// src/OrbitOS.Infrastructure/Persistence/Configurations/CanvasConfiguration.cs
public class CanvasConfiguration : IEntityTypeConfiguration<Canvas>
{
    public void Configure(EntityTypeBuilder<Canvas> builder)
    {
        builder.ToTable("canvases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.BlocksData)
            .HasColumnType("jsonb");

        // Relationships
        builder.HasOne(c => c.Organization)
            .WithMany(o => o.Canvases)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index per spec
        builder.HasIndex(c => new { c.OrganizationId, c.Name })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");
    }
}
```

### Step 4: Create Commands/Queries
```csharp
// src/OrbitOS.Application/Canvases/Commands/CreateCanvas.cs
public record CreateCanvasCommand(
    Guid OrganizationId,
    string Name,
    string? Description,
    CanvasType Type
) : IRequest<Result<CanvasDto>>;

public class CreateCanvasValidator : AbstractValidator<CreateCanvasCommand>
{
    public CreateCanvasValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}

public class CreateCanvasHandler : IRequestHandler<CreateCanvasCommand, Result<CanvasDto>>
{
    private readonly ICanvasRepository _repo;
    private readonly ICurrentUser _currentUser;

    public async Task<Result<CanvasDto>> Handle(
        CreateCanvasCommand request,
        CancellationToken ct)
    {
        var canvasResult = Canvas.Create(
            request.OrganizationId,
            request.Name,
            request.Type,
            _currentUser.Id);

        if (canvasResult.IsFailure)
            return Result.Failure<CanvasDto>(canvasResult.Error);

        await _repo.AddAsync(canvasResult.Value, ct);

        return canvasResult.Value.Adapt<CanvasDto>();
    }
}
```

### Step 5: Create API Endpoint
```csharp
// src/OrbitOS.Api/Controllers/CanvasesController.cs
[ApiController]
[Route("api/v1/organizations/{orgId}/canvases")]
[Authorize]
public class CanvasesController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost]
    [ProducesResponseType(typeof(CanvasDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<IActionResult> Create(
        Guid orgId,
        [FromBody] CreateCanvasRequest request,
        CancellationToken ct)
    {
        var command = new CreateCanvasCommand(
            orgId,
            request.Name,
            request.Description,
            request.Type);

        var result = await _mediator.Send(command, ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { orgId, id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }
}
```

### Step 6: Write Tests

```csharp
// tests/OrbitOS.UnitTests/Domain/CanvasTests.cs
public class CanvasTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        var result = Canvas.Create(
            Guid.NewGuid(),
            "Test Canvas",
            CanvasType.BusinessModel,
            Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        result.Value.State.Should().Be(CanvasState.Draft);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsFailure()
    {
        var result = Canvas.Create(
            Guid.NewGuid(),
            "",
            CanvasType.BusinessModel,
            Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
    }
}
```

## Running the API

```bash
# Development
dotnet run --project src/OrbitOS.Api

# With hot reload
dotnet watch run --project src/OrbitOS.Api

# With specific environment
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/OrbitOS.Api
```

## Running Tests

```bash
# All tests
dotnet test

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Specific project
dotnet test tests/OrbitOS.UnitTests

# With verbosity
dotnet test --verbosity normal
```

## Database Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate -p src/OrbitOS.Infrastructure -s src/OrbitOS.Api

# Apply migration
dotnet ef database update -p src/OrbitOS.Infrastructure -s src/OrbitOS.Api

# Generate SQL script
dotnet ef migrations script -p src/OrbitOS.Infrastructure -s src/OrbitOS.Api
```

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__Default` | PostgreSQL connection string | - |
| `Jwt__Secret` | JWT signing key | - |
| `Jwt__Issuer` | JWT issuer | https://api.orbitos.io |
| `Jwt__Audience` | JWT audience | orbitos |
| `Claude__ApiKey` | Anthropic API key | - |
| `Redis__ConnectionString` | Redis connection | localhost:6379 |

## Traceability

When implementing, always add traceability comments:

```csharp
/// <summary>
/// Canvas entity representing a strategic framework visualization.
/// </summary>
/// <remarks>
/// Specification: ENT-005 (specs/L4-data/entities/ENT-005-canvas.json)
/// Feature: F1 - Canvas Management
/// Validation Rules: R-CVS-001, R-CVS-002
/// </remarks>
public class Canvas : BaseEntity
```

## Security Checklist (SOC 2)

Before any PR:
- [ ] All inputs validated with FluentValidation
- [ ] No secrets in code or logs
- [ ] Audit logging for sensitive operations
- [ ] Multi-tenancy enforced (OrganizationId scope)
- [ ] Rate limiting on endpoints (SEC-CC6-07)
- [ ] HTTPS enforced
- [ ] JWT properly validated
