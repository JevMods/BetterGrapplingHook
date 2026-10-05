# BetterGrapplingHook

Tired of your grappling hook pulling you halfway through or not pulling at all? Tired of slipping off the surface every time you reach it or having to manually retrieve the anchor? This mod is for you! Heavily inspired by Just Cause game series.

## Features

- Small lift-off at launch.
- Pulling no longer stops early.
- Hangs on any surface and holds you in place.
- Stamina stops auto-regenerating while attached, and detaches when it runs out.

## Configuration

The settings are in `BepInEx/config/JevMods.BetterGrapplingHook.cfg`, created on first launch.

- `LaunchLift`: extra upward speed at launch, in m/s (default 6).
- `LiftDuration`: how long that extra speed lasts, in seconds (default 0.75).
- `OnArrival`: `Hang` keeps you attached, `AutoRetract` releases the hook (default `Hang`).

## Feedback

Found a bug or have an idea for a change? Open an issue on the [GitHub issues page](https://github.com/JevMods/BetterGrapplingHook/issues). Refactoring suggestions are welcome too. I'll go through everything as fast as I can.

## Building from source

To build you need Windows, the .NET SDK 8 or newer (with the .NET Framework 4.8 developer pack), Valheim and BepInEx. The easiest way to get BepInEx is to install BepInExPack_Valheim in a Thunderstore Mod Manager profile.

```
git clone https://github.com/JevMods/BetterGrapplingHook.git
cd BetterGrapplingHook
dotnet build src/BetterGrapplingHook -c Release
```

Then copy `src/BetterGrapplingHook/bin/Release/BetterGrapplingHook.dll` into `BepInEx/plugins`.

The build looks for Valheim through Steam and for BepInEx in the Default profile of the Thunderstore Mod Manager. If yours live elsewhere, pass the paths:

```
dotnet build src/BetterGrapplingHook -c Release -p:ValheimDir="D:\Games\Valheim" -p:BepInExDir="D:\Games\Valheim\BepInEx"
```

You can also set the `VALHEIM_INSTALL` environment variable. With no Valheim install at all, point `ManagedDir` at the `valheim_server_Data/Managed` folder of the free Valheim Dedicated Server (Steam app 896660). The GitHub workflow does exactly that.

## Notes

- Client-side: only the player who wants the effect needs to install it.
- Source: https://github.com/JevMods/BetterGrapplingHook
