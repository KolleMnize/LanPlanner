#!/usr/bin/env bash
set -euo pipefail

# Muss mit appsettings.Development.json übereinstimmen
SECRET="ein-langer-lokaler-testschluessel-mind-32-zeichen"
ISSUER="local-dev"
AUDIENCE="lanplanner-local"
EMAIL="${1:-test@example.com}"

b64() { openssl base64 -A | tr '+/' '-_' | tr -d '='; }

header=$(printf '{"alg":"HS256","typ":"JWT"}' | b64)
exp=$(( $(date +%s) + 3600 ))   # 1 Stunde gültig
payload=$(printf '{"iss":"%s","aud":"%s","email":"%s","exp":%s}' "$ISSUER" "$AUDIENCE" "$EMAIL" "$exp" | b64)
sig=$(printf '%s.%s' "$header" "$payload" | openssl dgst -sha256 -hmac "$SECRET" -binary | b64)

echo "$header.$payload.$sig"