#!/bin/bash
set -euo pipefail

WORKSPACE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG_DIR="$WORKSPACE_DIR/.logs"

API_URL="${API_URL:-http://localhost:5000/swagger/index.html}"
WEB_URL="${WEB_URL:-http://localhost:3000}"
WAIT_SECONDS="${WAIT_SECONDS:-60}"

cleanup() {
  "$WORKSPACE_DIR/scripts/stop-all.sh" >/dev/null 2>&1 || true
}

trap cleanup EXIT

echo "Starting services..."
"$WORKSPACE_DIR/scripts/start-all.sh" >/dev/null

echo "Waiting for API: $API_URL"
start_time="$(date +%s)"
while true; do
  if curl -fsS "$API_URL" >/dev/null 2>&1; then
    echo "API ready."
    break
  fi
  if [ $(( $(date +%s) - start_time )) -ge "$WAIT_SECONDS" ]; then
    echo "API did not become ready in ${WAIT_SECONDS}s."
    echo "API log:"
    tail -n 200 "$LOG_DIR/api.log" || true
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
    echo "Web log:"
    tail -n 200 "$LOG_DIR/web.log" || true
    exit 1
  fi
  sleep 2
done

if [ ! -d "$HOME/.cache/ms-playwright" ]; then
  echo "Installing Playwright browsers..."
  (cd "$WORKSPACE_DIR/orbitos-web" && npm run test:e2e:install)
fi

echo "Running Playwright e2e..."
(cd "$WORKSPACE_DIR/orbitos-web" && npm run test:e2e)
