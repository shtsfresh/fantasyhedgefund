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

  printf '   \033[1m%-15s\033[0mr    ' run
  printf '%-50s\n' "Start the server"

  printf '   \033[1m%-15s\033[0mu    ' update
  printf '%-50s\n' "Update dev dependencies"

  printf '   \033[1m%-15s\033[0mp    ' publish
  printf '%-50s\n' "Build production docker image"
  
  printf '   \033[1m%-15s\033[0m     ' deploy
  printf '%-50s\n' "Deploy current build to azure"

  echo 1>&2
}

if [[ "$NO_MOTD" != "true" ]]; then
  logo 1>&2
  menu 1>&2
fi

function update() {
  cd "$REPO_ROOT" || return 1
  ./bin/fantasy.sh update "$@"
}

function u() {
  update "$@"
}

function build() {
  cd "$REPO_ROOT" || return 1
  dotnet build "$@"
}

function b() {
  build "$@"
}

function start() {
  echo
  echo "Starting app at https://localhost:7209 ..."
  echo

  cd "$REPO_ROOT/Fantasy_Hedge_Fund" || return 1
  dotnet run "$@"
}

function s() {
  start "$@"
}

function publish() {
  cd "$REPO_ROOT" || return 1

  local currentBranch=""
  local tag=""

  if ! currentBranch="$(git rev-parse --abbrev-ref HEAD)" || [[ "$currentBranch" = "" ]]; then
    echo "Error fetching branch name" 1>&2
    return 1
  fi

  if [[ "$currentBranch" = "master" ]]; then
    tag="latest"
  else
    tag="$currentBranch"
  fi

  docker build -t fantasy-hedge-fund:"$tag" -f Dockerfile .
}

function p() {
  publish "$@"
}

function deploy() {
  local currentBranch=""
  local tag=""

  if ! currentBranch="$(git rev-parse --abbrev-ref HEAD)" || [[ "$currentBranch" = "" ]]; then
    echo "Error fetching branch name" 1>&2
    return 1
  fi

  case "$currentBranch" in
    master)
      tag="latest"
      ;;
    dev)
      tag="$currentBranch"
      ;;
    *)
      echo "Error: Only master and dev branches can be deployed" 1>&2
      return 1
      ;;
  esac

  az acr login --name fantasyhedgefund || return 1

  if ! docker tag fantasy-hedge-fund:"$tag" fantasyhedgefund.azurecr.io/app:"$tag"; then
    echo "Error: fantasy-hedge-fund:$tag not found. Have you run publish yet?" 1>&2
    return 1
  fi

  docker push fantasyhedgefund.azurecr.io/app:"$tag"
}