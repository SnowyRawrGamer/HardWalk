# Hard Walk

A BepInEx 6 IL2CPP mod skeleton for **Big Walk**.

## Features scaffolded

- Telescope shell game with three-button/box selection logic.
- Scaffold button repositioning to the crane.
- Blue House proximity-chat soundproofing/occlusion hook.
- Green buttons with a `Mathf.Sin` oscillating movement hook.

The Harmony patch targets and method signatures are intentionally placeholders: confirm the names and parameters against the current game's IL2CPP dump before enabling each patch.

## Build

Install the .NET 6 SDK and set `GameDir` to the Big Walk installation directory:

```bash
dotnet build HardWalk.csproj -p:GameDir="/path/to/Big Walk"
```

The project targets both `netstandard2.1` and `net6.0`. Copy the resulting `HardWalk.dll` into `BepInEx/plugins/HardWalk/`.

## Thunderstore package

Package `HardWalk.dll`, `manifest.json`, and this README in the package root. Do not include build intermediates or source files unless desired.

## Disclaimer

This is an educational starting point and is not affiliated with the developers or publisher of Big Walk. Back up saves and verify compatibility after game updates.
