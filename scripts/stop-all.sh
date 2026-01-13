#!/bin/bash
set -euo pipefail

WORKSPACE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG_DIR="$WORKSPACE_DIR/.logs"

echo "=========================================="
echo "Stopping OrbitOS Services"
echo "=========================================="

if [ -f "$LOG_DIR/web.pid" ]; then
  WEB_PID="$(cat "$LOG_DIR/web.pid")"
  if kill -0 "$WEB_PID" 2>/dev/null; then
    echo "Stopping Nuxt Web (PID: $WEB_PID)..."
    kill "$WEB_PID" || true
  fi
  rm -f "$LOG_DIR/web.pid"
fi

if [ -f "$LOG_DIR/api.pid" ]; then
  API_PID="$(cat "$LOG_DIR/api.pid")"
  if kill -0 "$API_PID" 2>/dev/null; then
    echo "Stopping .NET API (PID: $API_PID)..."
    kill "$API_PID" || true
  fi
  rm -f "$LOG_DIR/api.pid"
fi

echo "Stopping PostgreSQL..."
docker stop orbitos-postgres 2>/dev/null || echo "PostgreSQL not running or not configured"

echo "Stopping Redis..."
docker stop orbitos-redis 2>/dev/null || echo "Redis not running or not configured"

echo "=========================================="
echo "All Services Stopped"
echo "=========================================="
