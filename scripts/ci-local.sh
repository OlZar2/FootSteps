#!/usr/bin/env bash
set -euo pipefail

# Always teardown on exit (success or failure)
cleanup() {
  echo "Teardown..."
  docker compose -f ci-compose.yaml down -v >/dev/null 2>&1 || true
  rm -rf .secrets >/dev/null 2>&1 || true
}
trap cleanup EXIT

# Load local env (do not commit .env.local)
if [ -f .env.local ]; then
  set -a
  source .env.local
  set +a
fi

export SECRETS_DIR="$(pwd)/.secrets"

chmod +x ./scripts/fetch-selectel-secrets.sh
./scripts/fetch-selectel-secrets.sh

docker compose -f ci-compose.yaml build --pull
docker compose -f ci-compose.yaml up -d

# Wait for swagger
for i in {1..10}; do
  if curl -fsS http://localhost:5000/swagger/index.html >/dev/null 2>&1; then
    echo "OK: swagger is up"
    exit 0
  fi
  sleep 2
done

echo "ERROR: Swagger did not become available"
docker compose -f ci-compose.yaml ps
docker compose -f ci-compose.yaml logs --no-color --tail=200 fs-api
exit 1
