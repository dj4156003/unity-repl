#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd -- "$SCRIPT_DIR/../../.." && pwd)"

UNITY_REPL_DIR=""
for candidate in "$PROJECT_ROOT"/Packages/com.lambda-labs.unity-repl*; do
    if [[ -d "$candidate" ]]; then
        UNITY_REPL_DIR="$candidate"
        break
    fi
done

if [[ -z "$UNITY_REPL_DIR" ]]; then
    for candidate in "$PROJECT_ROOT"/Library/PackageCache/com.lambda-labs.unity-repl*; do
        if [[ -d "$candidate" ]]; then
            UNITY_REPL_DIR="$candidate"
            break
        fi
    done
fi

if [[ -z "$UNITY_REPL_DIR" ]]; then
    echo "ERROR: com.lambda-labs.unity-repl package not found under Packages or Library/PackageCache." >&2
    exit 3
fi

UNITY_REPL_SH="$UNITY_REPL_DIR/repl.sh"
if [[ ! -r "$UNITY_REPL_SH" ]]; then
    echo "ERROR: repl.sh is not readable: $UNITY_REPL_SH" >&2
    exit 3
fi

cd "$PROJECT_ROOT"

if [[ $# -eq 0 && -t 0 ]]; then
    tmp_script="${TMPDIR:-/tmp}/unity-repl-gs.$$.$RANDOM.sh"
    trap 'rm -f "$tmp_script"' EXIT
    tr -d '\r' < "$UNITY_REPL_SH" > "$tmp_script"
    bash "$tmp_script"
else
    tr -d '\r' < "$UNITY_REPL_SH" | bash -s -- "$@"
fi
