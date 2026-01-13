#!/bin/bash
# ============================================================================
# OrbitOS - Run All Tests
# ============================================================================
# Runs unit tests, integration tests, contract tests, and E2E tests
# Returns exit code 0 if all pass, non-zero otherwise
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

TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

print_header() {
    echo ""
    echo -e "${BLUE}=========================================="
    echo "$1"
    echo -e "==========================================${NC}"
}

print_result() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}[PASSED]${NC} $2"
        ((PASSED_TESTS++))
    else
        echo -e "${RED}[FAILED]${NC} $2"
        ((FAILED_TESTS++))
    fi
    ((TOTAL_TESTS++))
}

# ============================================================================
# Backend Tests (.NET)
# ============================================================================
print_header "1. Backend Tests (.NET)"

if [ -d "orbitos-api" ]; then
    cd orbitos-api

    # Unit Tests
    echo "Running unit tests..."
    if dotnet test tests/OrbitOS.UnitTests --no-build --verbosity quiet 2>/dev/null; then
        print_result 0 "Backend Unit Tests"
    else
        print_result 1 "Backend Unit Tests"
    fi

    # Integration Tests
    echo "Running integration tests..."
    if dotnet test tests/OrbitOS.IntegrationTests --no-build --verbosity quiet 2>/dev/null; then
        print_result 0 "Backend Integration Tests"
    else
        print_result 1 "Backend Integration Tests"
    fi

    # Architecture Tests
    echo "Running architecture tests..."
    if dotnet test tests/OrbitOS.ArchTests --no-build --verbosity quiet 2>/dev/null; then
        print_result 0 "Backend Architecture Tests"
    else
        print_result 1 "Backend Architecture Tests"
    fi

    cd "$WORKSPACE_DIR"
else
    echo -e "${YELLOW}[SKIP]${NC} Backend not found"
fi

# ============================================================================
# Web Frontend Tests (Nuxt/Vitest)
# ============================================================================
print_header "2. Web Frontend Tests (Nuxt)"

if [ -d "orbitos-web" ]; then
    cd orbitos-web

    # Unit Tests
    echo "Running unit tests..."
    if npm run test:unit -- --run 2>/dev/null; then
        print_result 0 "Web Unit Tests"
    else
        print_result 1 "Web Unit Tests"
    fi

    # Component Tests
    echo "Running component tests..."
    if npm run test:component -- --run 2>/dev/null; then
        print_result 0 "Web Component Tests"
    else
        print_result 1 "Web Component Tests"
    fi

    cd "$WORKSPACE_DIR"
else
    echo -e "${YELLOW}[SKIP]${NC} Web frontend not found"
fi

# ============================================================================
# Mobile Tests (Flutter)
# ============================================================================
print_header "3. Mobile Tests (Flutter)"

if [ -d "orbitos-mobile" ] && command -v flutter &> /dev/null; then
    cd orbitos-mobile

    # Unit Tests
    echo "Running unit tests..."
    if flutter test test/unit/ 2>/dev/null; then
        print_result 0 "Mobile Unit Tests"
    else
        print_result 1 "Mobile Unit Tests"
    fi

    # Widget Tests
    echo "Running widget tests..."
    if flutter test test/widget/ 2>/dev/null; then
        print_result 0 "Mobile Widget Tests"
    else
        print_result 1 "Mobile Widget Tests"
    fi

    cd "$WORKSPACE_DIR"
else
    echo -e "${YELLOW}[SKIP]${NC} Mobile app not found or Flutter not installed"
fi

# ============================================================================
# Contract Tests (OpenAPI Validation)
# ============================================================================
print_header "4. Contract Tests"

# Validate OpenAPI spec
echo "Validating OpenAPI specification..."
if [ -f "contracts/openapi.yaml" ]; then
    if command -v npx &> /dev/null; then
        if npx @redocly/cli lint contracts/openapi.yaml --skip-rule no-unused-components 2>/dev/null; then
            print_result 0 "OpenAPI Specification"
        else
            print_result 1 "OpenAPI Specification"
        fi
    else
        echo -e "${YELLOW}[SKIP]${NC} npx not available for OpenAPI validation"
    fi
fi

# ============================================================================
# E2E Tests (Playwright)
# ============================================================================
print_header "5. E2E Tests"

if [ -d "orbitos-web" ]; then
    cd orbitos-web

    echo "Running E2E tests..."
    if npm run test:e2e 2>/dev/null; then
        print_result 0 "E2E Tests"
    else
        print_result 1 "E2E Tests"
    fi

    cd "$WORKSPACE_DIR"
else
    echo -e "${YELLOW}[SKIP]${NC} E2E tests require web frontend"
fi

# ============================================================================
# Security Tests
# ============================================================================
print_header "6. Security Tests"

# Dependency vulnerability scan
echo "Scanning dependencies for vulnerabilities..."

if [ -d "orbitos-api" ]; then
    cd orbitos-api
    if dotnet list package --vulnerable --include-transitive 2>/dev/null | grep -q "has no vulnerable"; then
        print_result 0 "Backend Dependency Security"
    else
        print_result 1 "Backend Dependency Security"
    fi
    cd "$WORKSPACE_DIR"
fi

if [ -d "orbitos-web" ]; then
    cd orbitos-web
    if npm audit --audit-level=high 2>/dev/null; then
        print_result 0 "Web Dependency Security"
    else
        print_result 1 "Web Dependency Security"
    fi
    cd "$WORKSPACE_DIR"
fi

# ============================================================================
# Summary
# ============================================================================
print_header "Test Summary"

echo ""
echo "Total Tests:  $TOTAL_TESTS"
echo -e "Passed:       ${GREEN}$PASSED_TESTS${NC}"
echo -e "Failed:       ${RED}$FAILED_TESTS${NC}"
echo ""

if [ $FAILED_TESTS -eq 0 ]; then
    echo -e "${GREEN}All tests passed!${NC}"
    exit 0
else
    echo -e "${RED}Some tests failed. Please review the output above.${NC}"
    exit 1
fi
