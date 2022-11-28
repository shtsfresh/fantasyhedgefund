export REPO_ROOT="$(git rev-parse --show-superproject-working-tree --show-toplevel 2> /dev/null | head -1)"
export PS1="\001\033[38;5;214m\002\w\001\033[0m\002 \001\033[2m\002>\001\033[0m\002 "
export PATH="${REPO_ROOT}/.nix/bin:$PATH"

if [[ "$NO_MOTD" != "true" ]]; then
  printf '\033[38;5;214m' 1>&2
fi

function logo() {
  cat <<EOF

fantasy hedge fund
EOF
}

function menu() {
  printf '\033[0m\n'
  echo -e ' \033[2mCommands\033[0m\n'

  printf '   \033[1m%-15s\033[0m     ' menu
  printf '%-50s\n' "Show this menu"

  printf '   \033[1m%-15s\033[0mb    ' build
  printf '%-50s\n' "Build fantasy hedge fund"

  printf '   \033[1m%-15s\033[0ms    ' start
  printf '%-50s\n' "Start the server"

  printf '   \033[1m%-15s\033[0mu    ' update
  printf '%-50s\n' "Update the nix environment"
  echo 1>&2
}

if [[ "$NO_MOTD" != "true" ]]; then
  logo 1>&2
  menu 1>&2
fi

# Update the nix env
function update() {
  local procs

  if ! procs="$(nproc 2> /dev/null)" || [[ "$procs" -lt 1 ]]; then
    procs="1"
  fi

  rm "$REPO_ROOT/flake.lock"
  nix --no-warn-dirty --experimental-features 'nix-command flakes' build --max-jobs "$procs" --out-link "$REPO_ROOT/.nix"
}

function u() {
  update "$@"
}

function build() {
  cd "$REPO_ROOT/Fantasy_Hedge_Fund" && dotnet build "$@"
}

function start() {
  cd "$REPO_ROOT/Fantasy_Hedge_Fund" && dotnet run "$@"
}

function b() {
  build "$@"
}

function start() {
  cd "$REPO_ROOT/packages/web" || return 1
  pnpm run start
}

function s() {
  start "$@"
}
