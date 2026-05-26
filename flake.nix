{
  description = "RailHexLib — C# game logic library";
  inputs = {
    nixpkgs.url = "nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };
  outputs = {
    nixpkgs,
    flake-utils,
    ...
  }:
    flake-utils.lib.eachDefaultSystem (
      system: let
        pkgs = import nixpkgs {inherit system;};
        projectFile = "RailHexLib/RailHexLib.csproj";
        dotnet-sdk = pkgs.dotnet-sdk_8;
        dotnet-runtime = pkgs.dotnetCorePackages.runtime_8_0;
        version = "0.1.0";

        railHexLib = pkgs.buildDotnetModule {
          inherit projectFile dotnet-sdk dotnet-runtime version;
          pname = "RailHexLib";
          src = ./.;
          nugetDeps = ./nix/deps.nix;
          executables = [];
          # Тесты собираются отдельно (`dotnet test` в Tests/);
          # их пакеты требуют апдейта и блокируют сборку библиотеки.
          doCheck = false;

          # dotnet-tools.json (csharpier) нужен только для dev-окружения.
          # В nix-сборке его восстановление падает, т.к. csharpier не входит
          # в nugetDeps библиотеки. Убираем перед фазой configure.
          prePatch = ''
            rm -rf .config
          '';

          # Кладём собранную DLL в $out/lib, чтобы её мог подхватить
          # Godot-проект через ссылку (см. Makefile основного репо).
          # Путь содержит RID (например, net8.0/linux-x64/), поэтому ищем
          # рекурсивно — берём первую найденную DLL/PDB.
          installPhase = ''
            runHook preInstall
            mkdir -p $out/lib
            dll=$(find RailHexLib/bin/Release -name RailHexLib.dll -print -quit)
            pdb=$(find RailHexLib/bin/Release -name RailHexLib.pdb -print -quit || true)
            cp "$dll" $out/lib/
            if [ -n "$pdb" ]; then cp "$pdb" $out/lib/; fi
            runHook postInstall
          '';
        };
      in {
        packages = {
          default = railHexLib;
          library = railHexLib;
        };
        devShells = {
          default = pkgs.mkShell {
            buildInputs = [
              dotnet-sdk
              pkgs.git
              pkgs.alejandra
              pkgs.nodePackages.markdown-link-check
            ];
          };
        };
      }
    );
}
