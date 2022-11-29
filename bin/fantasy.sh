#!/usr/bin/env bash
set -eo pipefail

FANTASY_COMMAND_SRC=""

# shellcheck disable=SC2128
if [ -z "$BASH_SOURCE" ]; then
    FANTASY_COMMAND_SRC=${(%):-%N}
else
    FANTASY_COMMAND_SRC=${BASH_SOURCE[0]}
fi

# shellcheck disable=SC2155
export REPO_ROOT="$(dirname "$FANTASY_COMMAND_SRC")/../"

function update() {
  cd "$REPO_ROOT" || return 1

  if ! command -v nix > /dev/null 2> /dev/null; then
    echo -e '\033[1;33m[!] Error: nix is not installed\033[0m' 1>&2
    echo -e '\n fantasy hedge fund requires nix to run\nhttps://nixos.org/download.html#download-nix' 1>&2
    return 1
  fi

  local procs

  if ! procs="$(nproc 2> /dev/null)" || [[ "$procs" -lt 1 ]]; then
    procs="1"
  fi

  rm "$REPO_ROOT/flake.lock"
  nix --no-warn-dirty --experimental-features 'nix-command flakes' build --max-jobs "$procs" --out-link "$REPO_ROOT/.nix" || return 1

  # shellcheck disable=SC2016
  /usr/bin/env NO_MOTD=true PATH="${REPO_ROOT}/.nix/bin:$PATH" "$REPO_ROOT/.nix/bin/bash" -i -c \
    '. "$REPO_ROOT/.bashrc"; cd "$REPO_ROOT" && dotnet restore'
}

if [[ "$1" = "update" ]] || [[ "$1" = "u" ]]; then
  update || exit 1
  exit 0

elif ! [ -d "$REPO_ROOT/.nix" ]; then
  update || exit 1
fi

if [[ "$#" -gt 0 ]]; then
  # shellcheck disable=SC2016
  /usr/bin/env \
    NO_MOTD=true PATH="${REPO_ROOT}/.nix/bin:$PATH" \
    "$REPO_ROOT/.nix/bin/bash" -i -c '. "$REPO_ROOT/.bashrc"; command="$1"; shift; "$command" "$@"' bash "$@"

else
  "$REPO_ROOT/.nix/bin/bash" --rcfile "$REPO_ROOT/.bashrc"
fi
