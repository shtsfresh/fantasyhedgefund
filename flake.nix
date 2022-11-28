{
  description = "fantasy-hedge-fund";

  inputs = {
    nixpkgs.url = "http://nixos.org/channels/nixos-22.05/nixexprs.tar.xz";
    flake-utils.url = "github:numtide/flake-utils";
    devshell.url = "github:numtide/devshell/master";
  };

  outputs = { self, nixpkgs, devshell, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
       let
         pkgs = import nixpkgs {
           inherit system;
           overlays = [ devshell.overlay ];
         };

         deps = import ./dependencies.nix { inherit system; nixpkgs = pkgs; };

       in {
         packages.default = import ./default.nix { inherit system; nixpkgs = pkgs; };

         devShells.default = pkgs.mkShell {
           packages = deps;
           shellHook = (builtins.readFile ./.bashrc);
         };
       }
    );
}
