# ROTTR Mac Mod Manager

This is a macOS compatibility port of arcusmaximus's TR Reboot Mod Tools.
It adds support for Feral's native Steam layout while retaining the upstream
version-independent TIGER archive installer.

## Requirements

- The native macOS Steam release of Rise of the Tomb Raider
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) to build
  the C# 14 source. The resulting application targets the .NET 8 runtime.

## Build and run

From the repository root:

```sh
dotnet build ModManager/ModManager.csproj -c Debug
dotnet Build/Debug/TrRebootTools.ModManager.dll Rise
```

The second command can also be saved in a `.command` file for launching from
Finder. Running the application requires the .NET 8 runtime, which is included
with the .NET 10 SDK.

The manager automatically targets:

`~/Library/Application Support/Steam/steamapps/common/Rise of the Tomb Raider/RiseOfTheTombRaiderData`

Choose **Add folder**, then select the folder containing the unpacked mod.

The installer creates new `bigfile._mod.*.tiger` and matching `.nfo` files. It
does not overwrite the original TIGER archives. Remove or disable the mod from
the same manager if the native Feral executable does not load it correctly.

## Validation performed

The read-only `MacAudit` utility has been tested against the native Mac game
archives and can verify that a mod's resources resolve before installation:

```sh
dotnet run --project MacAudit -- \
  "$HOME/Library/Application Support/Steam/steamapps/common/Rise of the Tomb Raider/RiseOfTheTombRaiderData" \
  /path/to/unpacked-mod
```

## Source changes

- Added native macOS Steam discovery and Feral data-folder handling.
- Normalized macOS paths to the Windows separators used for CDC hashes.
- Stored settings under the user's config directory.
- Bundled the ROTTR filename lookup table with the manager.
- Updated SharpCompress to 0.48.0 and adapted to its current API.

The upstream project and this port are licensed under the MIT License; see
`LICENSE`.
