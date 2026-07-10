#!/usr/bin/env bash
#
# deploy.sh — repeatable deploy for the CRE backend Lambda functions.
#
# The backend has no IaC; each function is a single-file handler. AWS expects the
# entry point at index.handler, but in this repo the source file is named app.mjs.
# This script packages <handler-dir>/app.mjs as index.mjs and pushes it via
# `aws lambda update-function-code`.
#
# The Node.js 20 Lambda runtime already provides @aws-sdk/* (Secrets Manager, SSM,
# etc.), so no node_modules bundling is required.
#
# Written for portability against bash 3.2 (the default on macOS) — no associative
# arrays.
#
# Usage:
#   ./deploy.sh v2           # deploy TokenRetreivalHandler.V2 -> CRE-Backend-V2-TokenRetrievalHandler
#   ./deploy.sh redirect     # deploy RedirectionHandler       -> CRE-Backend-RedirectionHandler
#   ./deploy.sh all          # deploy all live functions
#
# Env overrides:
#   AWS_REGION   (default: us-east-1)
#   DRY_RUN=1    package only, do not upload

set -euo pipefail

REGION="${AWS_REGION:-us-east-1}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

resolve_src_dir() {
  case "$1" in
    v2)       echo "ChaosRecipeEnhancer.Backend.TokenRetreivalHandler.V2" ;;
    redirect) echo "ChaosRecipeEnhancer.Backend.RedirectionHandler" ;;
    *)        return 1 ;;
  esac
}

resolve_fn_name() {
  case "$1" in
    v2)       echo "CRE-Backend-V2-TokenRetrievalHandler" ;;
    redirect) echo "CRE-Backend-RedirectionHandler" ;;
    *)        return 1 ;;
  esac
}

deploy_one() {
  key="$1"
  src_dir="$(resolve_src_dir "$key")" || { echo "ERROR: unknown target '$key' (valid: v2 redirect all)" >&2; exit 1; }
  fn_name="$(resolve_fn_name "$key")"
  src_file="$SCRIPT_DIR/$src_dir/app.mjs"

  if [ ! -f "$src_file" ]; then
    echo "ERROR: source file not found: $src_file" >&2
    exit 1
  fi

  workdir="$(mktemp -d)"
  zip="$(mktemp -u).zip"

  # AWS entry point is index.handler; rename app.mjs -> index.mjs
  cp "$src_file" "$workdir/index.mjs"
  ( cd "$workdir" && zip -q -r "$zip" index.mjs )

  echo "==> $key: packaged $src_dir/app.mjs as index.mjs ($(wc -c < "$zip" | tr -d ' ') bytes)"

  if [ "${DRY_RUN:-0}" = "1" ]; then
    echo "    DRY_RUN=1 — skipping upload to $fn_name"
    rm -rf "$workdir" "$zip"
    return 0
  fi

  echo "    Uploading to Lambda function: $fn_name (region: $REGION)"
  aws lambda update-function-code \
    --region "$REGION" \
    --function-name "$fn_name" \
    --zip-file "fileb://$zip" \
    --publish \
    --query '{Function:FunctionName,Version:Version,LastModified:LastModified,CodeSize:CodeSize}' \
    --output table

  rm -rf "$workdir" "$zip"
}

main() {
  if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <v2|redirect|all>" >&2
    exit 1
  fi

  if [ "$1" = "all" ]; then
    for k in v2 redirect; do
      deploy_one "$k"
    done
  else
    deploy_one "$1"
  fi

  echo "Done."
}

main "$@"
