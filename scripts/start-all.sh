#!/bin/bash
# ============================================================================
# OrbitOS - Start All Services
# ============================================================================
# Starts all development services in the background with proper logging
# ============================================================================

set -e

WORKSPACE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$WORKSPACE_DIR"

LOG_DIR="$WORKSPACE_DIR/.logs"
mkdir -p "$LOG_DIR"

echo "=========================================="
echo "Starting OrbitOS Services"
echo "=========================================="

# Start PostgreSQL
echo "Starting PostgreSQL..."
docker start orbitos-postgres 2>/dev/null || echo "PostgreSQL already running or not configured"

# Start Redis
echo "Starting Redis..."
docker start orbitos-redis 2>/dev/null || echo "Redis already running or not configured"

# Wait for database to be ready
echo "Waiting for database..."
sleep 2

# Start .NET API
if [ -d "orbitos-api/src/OrbitOS.Api" ]; then
    echo "Starting .NET API on port 5000..."
    cd orbitos-api/src/OrbitOS.Api
    dotnet run --urls "http://localhost:5000" > "$LOG_DIR/api.log" 2>&1 &
    echo $! > "$LOG_DIR/api.pid"
    cd "$WORKSPACE_DIR"
    echo "  PID: $(cat $LOG_DIR/api.pid)"
    echo "  Log: $LOG_DIR/api.log"
fi

# Start Nuxt Web
if [ -d "orbitos-web" ]; then
    echo "Starting Nuxt Web on port 3000..."
    cd orbitos-web
    npm run dev > "$LOG_DIR/web.log" 2>&1 &
    echo $! > "$LOG_DIR/web.pid"
    cd "$WORKSPACE_DIR"
    echo "  PID: $(cat $LOG_DIR/web.pid)"
    echo "  Log: $LOG_DIR/web.log"
fi

echo ""
echo "=========================================="
echo "All Services Started"
echo "=========================================="
echo ""
echo "Service URLs:"
echo "  API:  http://localhost:5000"
echo "  Web:  http://localhost:3000"
echo ""
echo "To view logs:"
echo "  tail -f .logs/api.log"
echo "  tail -f .logs/web.log"
echo ""
echo "To stop all services:"
echo "  ./scripts/stop-all.sh"
echo ""
