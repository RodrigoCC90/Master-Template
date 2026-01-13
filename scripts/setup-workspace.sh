#!/bin/bash
# ============================================================================
# OrbitOS Workspace Setup Script
# ============================================================================
# This script sets up the complete development environment for AI-assisted
# development. Run this once when cloning the workspace.
# ============================================================================

set -e

echo "=========================================="
echo "OrbitOS Workspace Setup"
echo "=========================================="

WORKSPACE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$WORKSPACE_DIR"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${GREEN}[OK]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# ============================================================================
# 1. Check Prerequisites
# ============================================================================
echo ""
echo "1. Checking Prerequisites..."

# Check .NET SDK
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    print_status ".NET SDK: $DOTNET_VERSION"
else
    print_error ".NET SDK not found. Install from https://dotnet.microsoft.com/download"
    exit 1
fi

# Check Node.js
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    print_status "Node.js: $NODE_VERSION"
else
    print_error "Node.js not found. Install from https://nodejs.org/"
    exit 1
fi

# Check Flutter
if command -v flutter &> /dev/null; then
    FLUTTER_VERSION=$(flutter --version | head -n 1)
    print_status "Flutter: $FLUTTER_VERSION"
else
    print_warning "Flutter not found. Mobile development will be unavailable."
fi

# Check Docker
if command -v docker &> /dev/null; then
    DOCKER_VERSION=$(docker --version)
    print_status "Docker: $DOCKER_VERSION"
else
    print_warning "Docker not found. Integration tests require Docker."
fi

# Check PostgreSQL (optional - can use Docker)
if command -v psql &> /dev/null; then
    PSQL_VERSION=$(psql --version)
    print_status "PostgreSQL: $PSQL_VERSION"
else
    print_warning "PostgreSQL CLI not found. Will use Docker for database."
fi

# ============================================================================
# 2. Create Symlink to Specs
# ============================================================================
echo ""
echo "2. Setting up specs symlink..."

SPECS_SOURCE="../Operations-Tool/docs/srs"
if [ -d "$SPECS_SOURCE" ]; then
    if [ ! -L "specs" ]; then
        ln -s "$SPECS_SOURCE" specs
        print_status "Created symlink: specs -> $SPECS_SOURCE"
    else
        print_status "Symlink already exists: specs"
    fi
else
    print_warning "Specs directory not found at $SPECS_SOURCE"
    print_warning "Please ensure Operations-Tool repo is cloned alongside this workspace"
fi

# ============================================================================
# 3. Setup Backend (.NET)
# ============================================================================
echo ""
echo "3. Setting up .NET Backend..."

if [ -d "orbitos-api" ]; then
    cd orbitos-api
    dotnet restore
    print_status "Backend dependencies restored"
    cd ..
else
    print_warning "orbitos-api directory not found. Run create-backend.sh first."
fi

# ============================================================================
# 4. Setup Web Frontend (Nuxt)
# ============================================================================
echo ""
echo "4. Setting up Nuxt Web Frontend..."

if [ -d "orbitos-web" ]; then
    cd orbitos-web
    npm install
    print_status "Web frontend dependencies installed"
    cd ..
else
    print_warning "orbitos-web directory not found. Run create-web.sh first."
fi

# ============================================================================
# 5. Setup Mobile App (Flutter)
# ============================================================================
echo ""
echo "5. Setting up Flutter Mobile App..."

if [ -d "orbitos-mobile" ] && command -v flutter &> /dev/null; then
    cd orbitos-mobile
    flutter pub get
    print_status "Mobile dependencies installed"
    cd ..
else
    print_warning "orbitos-mobile directory not found or Flutter not installed."
fi

# ============================================================================
# 6. Setup Development Database
# ============================================================================
echo ""
echo "6. Setting up Development Database..."

if command -v docker &> /dev/null; then
    # Check if container already exists
    if docker ps -a | grep -q orbitos-postgres; then
        print_status "PostgreSQL container already exists"
    else
        docker run -d \
            --name orbitos-postgres \
            -e POSTGRES_USER=orbitos \
            -e POSTGRES_PASSWORD=orbitos_dev \
            -e POSTGRES_DB=orbitos_dev \
            -p 5432:5432 \
            postgres:15-alpine
        print_status "PostgreSQL container created"
    fi

    # Start container if stopped
    docker start orbitos-postgres 2>/dev/null || true
    print_status "PostgreSQL container running on port 5432"
else
    print_warning "Docker not available. Please set up PostgreSQL manually."
fi

# ============================================================================
# 7. Setup Redis (for caching)
# ============================================================================
echo ""
echo "7. Setting up Redis..."

if command -v docker &> /dev/null; then
    if docker ps -a | grep -q orbitos-redis; then
        print_status "Redis container already exists"
    else
        docker run -d \
            --name orbitos-redis \
            -p 6379:6379 \
            redis:7-alpine
        print_status "Redis container created"
    fi

    docker start orbitos-redis 2>/dev/null || true
    print_status "Redis container running on port 6379"
fi

# ============================================================================
# 8. Generate AI Context Files
# ============================================================================
echo ""
echo "8. Generating AI Context Files..."

# Generate entity summary for AI
if [ -d "specs/L4-data/entities" ]; then
    node scripts/generate-ai-context.js 2>/dev/null || print_warning "AI context generation script not found"
fi

# ============================================================================
# Summary
# ============================================================================
echo ""
echo "=========================================="
echo "Setup Complete!"
echo "=========================================="
echo ""
echo "Next steps:"
echo "  1. Start all services:  ./scripts/start-all.sh"
echo "  2. Run all tests:       ./scripts/test-all.sh"
echo "  3. Validate specs:      ./scripts/validate-all.sh"
echo ""
echo "Service URLs:"
echo "  - API:     http://localhost:5000"
echo "  - Web:     http://localhost:3000"
echo "  - PgAdmin: http://localhost:5050 (if installed)"
echo ""
echo "Database connection:"
echo "  - Host: localhost"
echo "  - Port: 5432"
echo "  - User: orbitos"
echo "  - Pass: orbitos_dev"
echo "  - DB:   orbitos_dev"
echo ""
