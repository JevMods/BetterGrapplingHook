# BetterGrapplingHook

In vanilla the grappling hook often stops short of the anchor, and when you do arrive you slide down the wall while the rope stretches. This mod lifts you off the ground at launch, pulls you all the way to the anchor, and leaves you hanging below the hook until you let go. You can also have the hook release by itself when you arrive.

## Features

- A lift-off at launch.
- The pull no longer stops early.
- On walls and under beams you hang below the hook, upright and held in place.
- Stamina stops regenerating while you are hooked, and the hook lets go when it runs out.

## Configuration

The settings are in `BepInEx/config/JevMods.BetterGrapplingHook.cfg`, created on first launch.

- `LaunchLift`: extra upward speed at launch, in m/s (default 6).
- `LiftDuration`: how long that extra speed lasts, in seconds (default 0.75).
- `OnArrival`: `Hang` keeps you attached, `AutoRetract` releases the hook (default `Hang`).

## Notes

- Client-side: only the player who wants the effect needs to install it.
- Source: https://github.com/JevMods/BetterGrapplingHook
