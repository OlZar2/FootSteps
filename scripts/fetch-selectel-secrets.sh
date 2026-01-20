#!/usr/bin/env bash
set -euo pipefail

# ===== Required env (set in CI/CD) =====
: "${SEL_ACCOUNT_ID:?need SEL_ACCOUNT_ID (account number)}"
: "${SEL_PROJECT_NAME:?need SEL_PROJECT_NAME (project name)}"
: "${SEL_USER_NAME:?need SEL_USER_NAME (service user name)}"
: "${SEL_USER_PASSWORD:?need SEL_USER_PASSWORD (service user password)}"

# ===== Config =====
IDENTITY_URL="${IDENTITY_URL:-https://cloud.api.selcloud.ru/identity/v3}"
SECRETS_URL="${SECRETS_URL:-https://cloud.api.selcloud.ru/secrets-manager/v1}"
SECRETS_DIR="${SECRETS_DIR:-/opt/fs/secrets}"

# Map "selectel_secret_name -> file_name_on_disk"
# Подстрой под свои имена секретов в Selectel.
declare -A SECRET_MAP=(
  ["fs_pg_password"]="fs_pg_password"
  ["fs_db_conn"]="fs_db_conn"
  ["fs_jwt_secret"]="fs_jwt_secret"
  ["fs_s3_access_key"]="fs_s3_access_key"
  ["fs_s3_secret_key"]="fs_s3_secret_key"
  ["fs_yandex_api_key"]="fs_yandex_api_key"
  ["fs_rabbitmq_password"]="fs_rabbitmq_password"
  ["fs_grafana_admin_password"]="fs_grafana_admin_password"
  ["fs_rabbitmq_connection"]="fs_rabbitmq_connection"
)

mkdir -p "$SECRETS_DIR"
chmod 700 "$SECRETS_DIR"

# ---- obtain IAM project-scoped token (X-Subject-Token) ----
get_project_token() {
  local resp token
  resp="$(
    curl -sS -i -X POST \
      -H 'Content-Type: application/json' \
      -d '{
        "auth":{
          "identity":{
            "methods":["password"],
            "password":{
              "user":{
                "name":"'"$SEL_USER_NAME"'",
                "domain":{"name":"'"$SEL_ACCOUNT_ID"'"},
                "password":"'"$SEL_USER_PASSWORD"'"
              }
            }
          },
          "scope":{
            "project":{
              "name":"'"$SEL_PROJECT_NAME"'",
              "domain":{"name":"'"$SEL_ACCOUNT_ID"'"}
            }
          }
        }
      }' \
      "${IDENTITY_URL}/auth/tokens"
  )"

  token="$(printf '%s' "$resp" | awk -F': ' 'tolower($1)=="x-subject-token"{print $2}' | tr -d '\r')"
  if [[ -z "${token}" ]]; then
    echo "ERROR: failed to obtain IAM project token (X-Subject-Token is empty)" >&2
    exit 1
  fi
  printf '%s' "$token"
}

# ---- decode base64 safely (works on linux/mac) ----
b64decode() {
  # GNU base64 uses -d, BSD uses -D
  if base64 -d >/dev/null 2>&1 <<<"Zg=="; then
    base64 -d
  else
    base64 -D
  fi
}

# ---- fetch one secret, decode "value" field (base64) ----
fetch_secret_value() {
  local token="$1"
  local name="$2"

  local resp body code
  resp="$(curl -sS -H "X-Auth-Token: ${token}" -w '\n%{http_code}' "${SECRETS_URL}/${name}")"
  body="$(printf '%s' "$resp" | sed '$d')"
  code="$(printf '%s' "$resp" | tail -n 1)"

  if [[ "$code" != "200" ]]; then
    echo "ERROR: failed to fetch secret '${name}' (HTTP ${code})" >&2
    echo "Response body:" >&2
    echo "$body" >&2
    exit 1
  fi

  python - <<'PY' "$body"
import json, sys
obj = json.loads(sys.argv[1])

# 1) value at root
v = obj.get("value")
if v:
    print(v)
    raise SystemExit(0)

# 2) value inside version
ver = obj.get("version")
if isinstance(ver, dict):
    vv = ver.get("value")
    if vv:
        print(vv)
        raise SystemExit(0)

# otherwise: helpful error
keys = list(obj.keys()) if isinstance(obj, dict) else [type(obj).__name__]
ver_keys = list(ver.keys()) if isinstance(ver, dict) else [type(ver).__name__]
raise SystemExit(f"ERROR: neither 'value' nor 'version.value' found. Root keys={keys}, version keys={ver_keys}")
PY
}

write_secret_file() {
  local file_path="$1"
  local b64_value="$2"

  umask 077
  local tmp
  tmp="$(mktemp "${file_path}.tmp.XXXXXX")"

  # decode -> tmp
  printf '%s' "$b64_value" | b64decode > "$tmp"
  chmod 600 "$tmp"
  mv -f "$tmp" "$file_path"
}

main() {
  local token
  token="$(get_project_token)"

  for secret_name in "${!SECRET_MAP[@]}"; do
    local out_file="${SECRETS_DIR}/${SECRET_MAP[$secret_name]}"
    local b64
    b64="$(fetch_secret_value "$token" "$secret_name")"
    write_secret_file "$out_file" "$b64"
    echo "OK: wrote ${out_file}"
  done
}

main
