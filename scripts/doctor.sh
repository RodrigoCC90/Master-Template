#!/bin/bash
set -euo pipefail

WORKSPACE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG_DIR="$WORKSPACE_DIR/.logs"

API_URL="${API_URL:-http://localhost:5001/swagger/index.html}"
WEB_URL="${WEB_URL:-http://localhost:3001}"
WAIT_SECONDS="${WAIT_SECONDS:-90}"

echo "OrbitOS Doctor"
echo "=============="

if ! command -v docker >/dev/null 2>&1; then
  echo "Docker CLI not found. Install Docker Desktop and try again."
  exit 1
fi

if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon not running. Start Docker Desktop and try again."
  exit 1
fi

echo "Starting stack with Docker..."
docker compose up --build -d

cleanup() {
  echo "Stopping stack..."
  docker compose down --remove-orphans >/dev/null 2>&1 || true
}
trap cleanup EXIT

echo "Waiting for API: $API_URL"
start_time="$(date +%s)"
while true; do
  if curl -fsS "$API_URL" >/dev/null 2>&1; then
    echo "API ready."
    break
  fi
  if [ $(( $(date +%s) - start_time )) -ge "$WAIT_SECONDS" ]; then
    echo "API did not become ready in ${WAIT_SECONDS}s."
    echo "API logs:"
    docker compose logs --tail=200 api || true
    exit 1
  fi
  sleep 2
done

echo "Waiting for Web: $WEB_URL"
start_time="$(date +%s)"
while true; do
  if curl -fsS "$WEB_URL" >/dev/null 2>&1; then
    echo "Web ready."
    break
  fi
  if [ $(( $(date +%s) - start_time )) -ge "$WAIT_SECONDS" ]; then
    echo "Web did not become ready in ${WAIT_SECONDS}s."
    echo "Web logs:"
    docker compose logs --tail=200 web || true
    exit 1
  fi
  sleep 2
done

echo "Success. API and Web responded."
