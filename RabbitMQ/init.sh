#!/usr/bin/env bash
set -euo pipefail

RABBIT_HOST="raid-rabbit"
MGMT_PORT="15672"
VHOST="wynncraft"

ADMIN_USER="wynncraft"
ADMIN_PASS="$(cat /run/secrets/rabbitmq_admin_password)"

RAIDBOT_USER="raid-bot"
RAIDBOT_PASS="$(cat /run/secrets/rabbitmq_raidbot_password)"

WEBSITE_USER="wynncraft-website"
WEBSITE_PASS="$(cat /run/secrets/rabbitmq_website_password)"

EXCHANGE_NAME="raids.exchange"
QUEUE_NAME="raids.completed.q"
ROUTING_KEY="raid.completed"

# Fail fast if any secret is empty
for v in ADMIN_PASS RAIDBOT_PASS WEBSITE_PASS; do
  if [[ -z "${!v}" ]]; then
    echo "Secret ${v} is empty. Check /run/secrets mounts." >&2
    exit 1
  fi
done

api() {
  local method="$1"
  local path="$2"
  local data="${3:-}"

  if [[ -n "$data" ]]; then
    curl -fsS -u "${ADMIN_USER}:${ADMIN_PASS}" \
      -H "content-type: application/json" \
      -X "$method" "http://${RABBIT_HOST}:${MGMT_PORT}${path}" \
      -d "$data"
  else
    curl -fsS -u "${ADMIN_USER}:${ADMIN_PASS}" \
      -H "content-type: application/json" \
      -X "$method" "http://${RABBIT_HOST}:${MGMT_PORT}${path}"
  fi
}

echo "Waiting for RabbitMQ Management API (and admin creds) to become ready..."
until curl -fsS -u "${ADMIN_USER}:${ADMIN_PASS}" \
  "http://${RABBIT_HOST}:${MGMT_PORT}/api/overview" >/dev/null; do
  echo "  ...not ready yet"
  sleep 2
done

echo "Ensuring vhost '${VHOST}' exists..."
api PUT "/api/vhosts/${VHOST}" '{}'

echo "Creating/updating users..."
api PUT "/api/users/${ADMIN_USER}" \
  "{\"password\":\"${ADMIN_PASS}\",\"tags\":\"administrator\"}"

api PUT "/api/users/${RAIDBOT_USER}" \
  "{\"password\":\"${RAIDBOT_PASS}\",\"tags\":\"\"}"

api PUT "/api/users/${WEBSITE_USER}" \
  "{\"password\":\"${WEBSITE_PASS}\",\"tags\":\"\"}"

echo "Setting permissions..."
api PUT "/api/permissions/${VHOST}/${ADMIN_USER}" \
  "{\"configure\":\".*\",\"write\":\".*\",\"read\":\".*\"}"

api PUT "/api/permissions/${VHOST}/${RAIDBOT_USER}" \
  "{\"configure\":\"^$\",\"write\":\"^${EXCHANGE_NAME//./\\\\.}$\",\"read\":\"^$\"}"

api PUT "/api/permissions/${VHOST}/${WEBSITE_USER}" \
  "{\"configure\":\"^$\",\"write\":\"^$\",\"read\":\"^${QUEUE_NAME//./\\\\.}$\"}"

echo "Ensuring exchange exists..."
api PUT "/api/exchanges/${VHOST}/${EXCHANGE_NAME}" \
  "{\"type\":\"topic\",\"durable\":true,\"auto_delete\":false,\"internal\":false,\"arguments\":{}}"

echo "Ensuring queue exists..."
api PUT "/api/queues/${VHOST}/${QUEUE_NAME}" \
  "{\"durable\":true,\"auto_delete\":false,\"arguments\":{}}"

echo "Ensuring binding exists..."
api POST "/api/bindings/${VHOST}/e/${EXCHANGE_NAME}/q/${QUEUE_NAME}" \
  "{\"routing_key\":\"${ROUTING_KEY}\",\"arguments\":{}}"

echo "RabbitMQ init complete ✅"
