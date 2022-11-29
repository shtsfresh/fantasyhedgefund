# Fantasy Hedge Fund

#### Setup
1. Install nix: ```https://nixos.org/download.html```
2. Install docker desktop: ```https://www.docker.com/products/docker-desktop/```
3. cd into repo and fetch dependencies: ```./bin/fantasy.sh update```

#### Usage
```sh
# Open dev shell 
$ ./bin/fantasy.sh

# Update dev environment dependencies and dotnet dependencies
$ ./bin/fantasy.sh update

# Local build
$ ./bin/fantasy.sh build

# Run local build
$ ./bin/fantasy.sh run

# Production build
$ ./bin/fantasy.sh publish

# Deploy production build
$ ./bin/fantasy.sh deploy
```

#### Adding packages to the dev shell
Any packages added to ```./dependencies.nix``` will be installed automatically by ```./bin/fantasy.sh update``` 
and will available in the dev shell.

Search for packages here: https://search.nixos.org/packages

#### Todo
1. Get mssql container running for local development
2. Migrate contractor's database to our cloud and fix current db config
3. Remove hard-coded keys and passwords and manage with secret manager
4. Setup ci to auto-deploy pushes to master branch to prod instance
5. Setup dev instance
6. Setup ci to auto-deploy pushes to dev branch to dev instance