{ nixpkgs, system }:
let
  dependencies = import ./dependencies.nix { inherit nixpkgs system; };
in
  nixpkgs.buildEnv {
    name = "fantasy-hedge-fund";

    paths = [] ++ dependencies;

    pathsToLink = [
      "/bin"
      "/lib"
      "/etc"
    ];
  }