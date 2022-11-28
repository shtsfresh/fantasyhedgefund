#!/usr/bin/env bash
set -euo pipefail

# Get path to git repository root
# shellcheck disable=SC2155
export REPO_ROOT="$(git rev-parse --show-superproject-working-tree --show-toplevel 2> /dev/null | head -1)"

if ! command -v nix > /dev/null 2> /dev/null; then
  echo -e '\033[1;33m[!] Error: nix is not installed\033[0m' 1>&2
  echo -e '\n fantasy hedge fund requires nix to run\nhttps://nixos.org/download.html#download-nix' 1>&2
  exit 1
fi

if ! [ -d "$REPO_ROOT/.nix" ] && ! [ -L "$REPO_ROOT/.nix" ]; then
  (. "$REPO_ROOT/.bashrc" && update) 1>&2 || exit 1
fi

if [[ "$#" -gt 0 ]]; then
  # shellcheck disable=SC2016
  /usr/bin/env \
    NO_MOTD=true PATH="${REPO_ROOT}/.nix/bin:$PATH" \
    bash -i -c '. "$REPO_ROOT/.bashrc"; command="$1"; shift; "$command" "$@"' bash "$@"
else
  bash --rcfile "$REPO_ROOT/.bashrc"
fi