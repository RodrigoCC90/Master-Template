# OrbitOS Workspace - AI Development Guide

## Overview

This is the **OrbitOS monorepo workspace** containing all repositories for the AI-native business operating system. This file provides AI assistants with the context needed to develop, test, and validate across all projects.

## Workspace Structure

```
/OrbitOS-Workspace/
├── CLAUDE.md                    # This file - workspace-level AI instructions
├── .ai/                         # AI orchestration and context
│   ├── commands/                # AI-executable scripts
│   ├── context/                 # Generated context files
│   └── prompts/                 # Reusable prompt templates
├── contracts/                   # Shared API contracts (OpenAPI, Protobuf)
│   ├── openapi.yaml            # OpenAPI 3.1 specification
│   └── schemas/                # JSON schemas for validation
├── specs/                       # Symlink to Operations-Tool/docs/srs
├── orbitos-api/                 # .NET 8 Backend API
├── orbitos-web/                 # Nuxt 4 + Vuetify Web Frontend
└── orbitos-mobile/              # Flutter Mobile App
```

## Quick Reference

| Repo | Tech Stack | Port | Command to Run |
|------|------------|------|----------------|
| orbitos-api | .NET 8, EF Core, PostgreSQL | 5000 | `dotnet run` |
| orbitos-web | Nuxt 4, Vuetify 3, Pinia | 3000 | `npm run dev` |
| orbitos-mobile | Flutter 3.x, BLoC | N/A | `flutter run` |

## AI Development Protocol

### Before Writing Any Code

1. **Read the specification first**:
   ```
   specs/manifest.yaml           # AI navigation guide
   specs/L4-data/entities/       # Entity definitions
   specs/L3-domain/              # Business rules
   specs/nfr/                    # Non-functional requirements
   ```

2. **Check the API contract**:
   ```
   contracts/openapi.yaml        # All endpoints defined here
   ```

3. **Understand traceability**:
   - Every entity maps to: ENT-###
   - Every feature maps to: F#
   - Every requirement maps to: REQ-{LAYER}-{CATEGORY}-###

### Code Standards (Enforced Across All Repos)

| Standard | Requirement |
|----------|-------------|
| **Type Safety** | No `any` types (TypeScript), no `dynamic` (C#/Dart) |
| **Validation** | All inputs validated at boundaries |
| **Multi-Tenancy** | All queries MUST be scoped by `organization_id` |
| **Soft Delete** | Never hard delete - set `deleted_at` timestamp |
| **Audit Fields** | All entities: `created_at`, `updated_at`, `created_by`, `updated_by` |
| **Error Handling** | Result types for business logic, exceptions for infrastructure |

### Testing Requirements (ISO 29119 Aligned)

| Test Type | Coverage | Framework |
|-----------|----------|-----------|
| Unit Tests | 80% minimum | xUnit (.NET), Vitest (Nuxt), flutter_test |
| Integration Tests | 100% of APIs | TestContainers + actual DB |
| Contract Tests | 100% of endpoints | Pact / OpenAPI validation |
| E2E Tests | Critical paths | Playwright (Web), integration_test (Flutter) |
| Security Tests | OWASP Top 10 | OWASP ZAP, dependency scanning |
| Performance Tests | NFR compliance | k6, Lighthouse |

## AI Commands

The AI can execute these commands to run, test, and validate:

### Run All Services
```bash
# Terminal 1: Start API
cd orbitos-api && dotnet run

# Terminal 2: Start Web
cd orbitos-web && npm run dev

# Terminal 3: Start Mobile (iOS Simulator)
cd orbitos-mobile && flutter run
```

### Run All Tests
```bash
# API Tests
cd orbitos-api && dotnet test --collect:"XPlat Code Coverage"

# Web Tests
cd orbitos-web && npm run test:unit && npm run test:e2e

# Mobile Tests
cd orbitos-mobile && flutter test

# Contract Tests (validates all repos against OpenAPI)
npm run test:contracts
```

### Validate Against Specs
```bash
# Validate OpenAPI spec is complete
npm run validate:openapi

# Validate entities match database schema
npm run validate:entities

# Validate all traceability links
npm run validate:traceability
```

## Entity to Code Mapping

When implementing an entity from the specs:

| Spec Location | .NET Location | Nuxt Location | Flutter Location |
|--------------|---------------|---------------|------------------|
| `specs/L4-data/entities/ENT-###.json` | `src/OrbitOS.Domain/Entities/` | `app/types/entities/` | `lib/domain/entities/` |
| `specs/L3-domain/business-rules/` | `src/OrbitOS.Domain/Rules/` | `app/composables/rules/` | `lib/domain/rules/` |
| `specs/L3-domain/authorization/` | `src/OrbitOS.Domain/Authorization/` | `app/middleware/` | `lib/core/auth/` |
| `specs/nfr/NFR-SEC-security.json` | Cross-cutting | Cross-cutting | Cross-cutting |

## Feature Implementation Checklist

When implementing a feature (e.g., F1 - Canvas Management):

- [ ] Read `specs/features/F1-canvas-management.json`
- [ ] Identify required entities from feature spec
- [ ] Read each entity definition from `specs/L4-data/entities/`
- [ ] Identify business rules from `specs/L3-domain/`
- [ ] Check validation rules in `specs/L4-data/constraints/`
- [ ] Implement API endpoints per `contracts/openapi.yaml`
- [ ] Implement domain logic with business rules
- [ ] Write unit tests (80% coverage)
- [ ] Write integration tests (API contract)
- [ ] Write E2E tests (critical user flows)
- [ ] Update traceability in spec files

## Multi-Tenancy Implementation

**CRITICAL**: Every database query MUST be scoped by `organization_id`.

### .NET (EF Core)
```csharp
// Global query filter in DbContext
modelBuilder.Entity<Canvas>().HasQueryFilter(c => c.OrganizationId == _tenantId);
```

### Nuxt (API calls)
```typescript
// All API calls include org context from auth
const { data } = await $fetch(`/api/organizations/${orgId}/canvases`);
```

### Flutter (Repository)
```dart
// Organization context from auth state
final canvases = await canvasRepository.getAll(organizationId: authState.orgId);
```

## SOC 2 Compliance Checklist

Reference: `specs/L5-infrastructure/security/SEC-001-soc2-compliance.json`

Before any PR:
- [ ] No secrets in code (check SEC-CC8-01)
- [ ] Input validation on all endpoints (check SEC-PI1-02)
- [ ] Audit logging for security events (check SEC-CC4-01)
- [ ] Proper error handling (no stack traces to client)
- [ ] Rate limiting on public endpoints (check NFR-SEC-008)

## Getting Started for AI

1. **First time setup**:
   ```bash
   cd OrbitOS-Workspace
   ./scripts/setup-workspace.sh
   ```

2. **Start development**:
   ```bash
   ./scripts/start-all.sh
   ```

3. **Run full validation**:
   ```bash
   ./scripts/validate-all.sh
   ```

## Key Files to Read

| Purpose | File |
|---------|------|
| AI Navigation | `specs/manifest.yaml` |
| All Entities | `specs/L4-data/index.json` |
| Business Rules | `specs/L3-domain/index.json` |
| Security Requirements | `specs/nfr/NFR-SEC-security.json` |
| SOC 2 Controls | `specs/L5-infrastructure/security/SEC-002-controls-matrix.json` |
| API Contract | `contracts/openapi.yaml` |

## Version Compatibility

| Component | Version | Notes |
|-----------|---------|-------|
| .NET | 8.0 LTS | Long-term support |
| Node.js | 20 LTS | For Nuxt build |
| Flutter | 3.x stable | Latest stable |
| PostgreSQL | 15+ | JSON support required |
| Docker | 24+ | For TestContainers |
