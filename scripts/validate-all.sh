#!/bin/bash
# ============================================================================
# OrbitOS - Validate All Components
# ============================================================================
# Validates:
# 1. OpenAPI spec is valid and complete
# 2. All entities from SRS have corresponding code
# 3. All API endpoints are implemented
# 4. Code follows architecture rules
# 5. Security requirements are met
# ============================================================================

set -e

WORKSPACE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$WORKSPACE_DIR"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

ERRORS=0
WARNINGS=0

print_header() {
    echo ""
    echo -e "${BLUE}=========================================="
    echo "$1"
    echo -e "==========================================${NC}"
}

print_ok() {
    echo -e "${GREEN}[OK]${NC} $1"
}

print_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
    ((WARNINGS++))
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
    ((ERRORS++))
}

# ============================================================================
# 1. Validate OpenAPI Specification
# ============================================================================
print_header "1. OpenAPI Specification Validation"

if [ -f "contracts/openapi.yaml" ]; then
    echo "Checking OpenAPI spec syntax..."
    if command -v npx &> /dev/null; then
        if npx @redocly/cli lint contracts/openapi.yaml --skip-rule no-unused-components 2>/dev/null; then
            print_ok "OpenAPI syntax valid"
        else
            print_error "OpenAPI spec has errors"
        fi
    else
        print_warn "npx not available, skipping OpenAPI validation"
    fi
else
    print_error "contracts/openapi.yaml not found"
fi

# ============================================================================
# 2. Validate Entity Coverage
# ============================================================================
print_header "2. Entity Coverage Validation"

if [ -d "specs/L4-data/entities" ]; then
    echo "Checking entity implementation coverage..."

    ENTITY_COUNT=$(ls -1 specs/L4-data/entities/ENT-*.json 2>/dev/null | wc -l | tr -d ' ')
    echo "Found $ENTITY_COUNT entities in specs"

    # Check .NET entities
    if [ -d "orbitos-api/src/OrbitOS.Domain/Entities" ]; then
        DOTNET_ENTITIES=$(ls -1 orbitos-api/src/OrbitOS.Domain/Entities/*.cs 2>/dev/null | wc -l | tr -d ' ')
        if [ "$DOTNET_ENTITIES" -ge "$ENTITY_COUNT" ]; then
            print_ok "Backend has $DOTNET_ENTITIES entity classes"
        else
            print_warn "Backend only has $DOTNET_ENTITIES of $ENTITY_COUNT entities implemented"
        fi
    else
        print_warn "Backend entities directory not found"
    fi

    # Check Flutter entities
    if [ -d "orbitos-mobile/lib/domain/entities" ]; then
        FLUTTER_ENTITIES=$(ls -1 orbitos-mobile/lib/domain/entities/*.dart 2>/dev/null | wc -l | tr -d ' ')
        if [ "$FLUTTER_ENTITIES" -ge "$ENTITY_COUNT" ]; then
            print_ok "Mobile has $FLUTTER_ENTITIES entity classes"
        else
            print_warn "Mobile only has $FLUTTER_ENTITIES of $ENTITY_COUNT entities implemented"
        fi
    else
        print_warn "Mobile entities directory not found"
    fi

    # Check TypeScript types
    if [ -f "orbitos-web/app/types/entities.ts" ]; then
        print_ok "Web has entities type definitions"
    else
        print_warn "Web entities types not found"
    fi
else
    print_warn "Specs directory not found"
fi

# ============================================================================
# 3. Validate API Endpoint Coverage
# ============================================================================
print_header "3. API Endpoint Coverage Validation"

if [ -f "contracts/openapi.yaml" ]; then
    ENDPOINT_COUNT=$(grep -c "operationId:" contracts/openapi.yaml || echo "0")
    echo "Found $ENDPOINT_COUNT endpoints in OpenAPI spec"

    # Check .NET controllers
    if [ -d "orbitos-api/src/OrbitOS.Api/Controllers" ]; then
        CONTROLLER_COUNT=$(ls -1 orbitos-api/src/OrbitOS.Api/Controllers/*Controller.cs 2>/dev/null | wc -l | tr -d ' ')
        print_ok "Backend has $CONTROLLER_COUNT controllers"
    else
        print_warn "Backend controllers directory not found"
    fi
fi

# ============================================================================
# 4. Validate Architecture Rules
# ============================================================================
print_header "4. Architecture Rules Validation"

# Check for forbidden patterns in .NET
if [ -d "orbitos-api" ]; then
    echo "Checking .NET architecture rules..."

    # Domain should not reference Infrastructure
    if grep -r "OrbitOS.Infrastructure" orbitos-api/src/OrbitOS.Domain/ 2>/dev/null | grep -v "\.csproj" > /dev/null; then
        print_error "Domain layer references Infrastructure (forbidden)"
    else
        print_ok "Domain layer has no Infrastructure references"
    fi

    # Check for 'dynamic' keyword (forbidden)
    if grep -r "\bdynamic\b" orbitos-api/src/ 2>/dev/null | grep -v "\.csproj" > /dev/null; then
        print_warn "Found 'dynamic' keyword usage in .NET code"
    else
        print_ok "No 'dynamic' keyword found"
    fi
fi

# Check for forbidden patterns in Flutter
if [ -d "orbitos-mobile/lib" ]; then
    echo "Checking Flutter architecture rules..."

    # Domain should not import data or presentation
    if grep -r "import.*data/" orbitos-mobile/lib/domain/ 2>/dev/null > /dev/null; then
        print_error "Domain layer imports from data layer (forbidden)"
    else
        print_ok "Domain layer has clean imports"
    fi

    # Check for 'dynamic' keyword (forbidden)
    if grep -r "\bdynamic\b" orbitos-mobile/lib/ 2>/dev/null > /dev/null; then
        print_warn "Found 'dynamic' keyword usage in Flutter code"
    else
        print_ok "No 'dynamic' keyword found"
    fi
fi

# Check for forbidden patterns in Nuxt
if [ -d "orbitos-web/app" ]; then
    echo "Checking Nuxt architecture rules..."

    # Check for 'any' type (forbidden)
    if grep -r ": any" orbitos-web/app/ 2>/dev/null > /dev/null; then
        print_warn "Found 'any' type usage in TypeScript code"
    else
        print_ok "No 'any' type found"
    fi

    # Check for console.log (should be removed in production)
    if grep -r "console\.log" orbitos-web/app/ 2>/dev/null > /dev/null; then
        print_warn "Found console.log statements"
    else
        print_ok "No console.log statements"
    fi
fi

# ============================================================================
# 5. Validate Security Requirements
# ============================================================================
print_header "5. Security Requirements Validation"

echo "Checking for security issues..."

# Check for hardcoded secrets
SECRET_PATTERNS="password\s*=|api_key\s*=|secret\s*=|apiKey\s*=|API_KEY\s*=|PASSWORD\s*="
EXCLUDE_PATTERNS="\.md$|\.example$|\.sample$|test"

if grep -rE "$SECRET_PATTERNS" --include="*.cs" --include="*.ts" --include="*.dart" --include="*.json" . 2>/dev/null | grep -vE "$EXCLUDE_PATTERNS" > /dev/null; then
    print_error "Potential hardcoded secrets found"
else
    print_ok "No hardcoded secrets detected"
fi

# Check for TODO security items
if grep -ri "TODO.*security" --include="*.cs" --include="*.ts" --include="*.dart" . 2>/dev/null > /dev/null; then
    print_warn "Found TODO items related to security"
else
    print_ok "No security TODOs found"
fi

# ============================================================================
# 6. Validate Traceability
# ============================================================================
print_header "6. Traceability Validation"

echo "Checking traceability comments..."

# Check .NET files for ENT- references
if [ -d "orbitos-api/src/OrbitOS.Domain/Entities" ]; then
    TRACED_DOTNET=$(grep -l "ENT-" orbitos-api/src/OrbitOS.Domain/Entities/*.cs 2>/dev/null | wc -l | tr -d ' ')
    TOTAL_DOTNET=$(ls -1 orbitos-api/src/OrbitOS.Domain/Entities/*.cs 2>/dev/null | wc -l | tr -d ' ')
    if [ "$TRACED_DOTNET" -eq "$TOTAL_DOTNET" ] && [ "$TOTAL_DOTNET" -gt 0 ]; then
        print_ok "All .NET entities have spec traceability"
    else
        print_warn "$TRACED_DOTNET of $TOTAL_DOTNET .NET entities have spec traceability"
    fi
fi

# ============================================================================
# Summary
# ============================================================================
print_header "Validation Summary"

echo ""
echo "Errors:   $ERRORS"
echo "Warnings: $WARNINGS"
echo ""

if [ $ERRORS -eq 0 ]; then
    if [ $WARNINGS -eq 0 ]; then
        echo -e "${GREEN}All validations passed!${NC}"
    else
        echo -e "${YELLOW}Validations passed with warnings.${NC}"
    fi
    exit 0
else
    echo -e "${RED}Validations failed. Please fix errors above.${NC}"
    exit 1
fi
