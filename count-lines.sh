#!/usr/bin/env bash
#
# Count lines of C# source in the repository.
#
# Excludes build output (bin/, obj/) so only hand-written source is counted.
# Prints a per-assignment breakdown and a total.
#
# Usage:
#   ./count-lines.sh

set -euo pipefail

# Run from the repo root regardless of where the script is invoked from.
cd "$(dirname "$0")"

count_lines() {
    # $1: directory to search. Sums lines across all .cs files under it,
    # skipping bin/ and obj/. Prints 0 if there are no matching files.
    find "$1" -type f -name '*.cs' \
        -not -path '*/bin/*' -not -path '*/obj/*' -print0 \
        | xargs -0 cat 2>/dev/null \
        | wc -l \
        | tr -d ' '
}

a1=$(count_lines Assignment1)
a2=$(count_lines Assignment2)
total=$((a1 + a2))

printf '%-14s %6s\n' 'Assignment 1:' "$a1"
printf '%-14s %6s\n' 'Assignment 2:' "$a2"
printf '%-14s %6s\n' 'Total:' "$total"
