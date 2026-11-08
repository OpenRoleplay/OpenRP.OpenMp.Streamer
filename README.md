# OpenRP.OpenMp.Streamer

The [Incognito streamer](https://github.com/samp-incognito/samp-streamer-plugin) for SampSharp 1.x gamemodes on open.mp x64, with every streamer item as an **ECS component**. Create far more objects, pickups, text labels, map icons, checkpoints and actors than the game allows, and let the streamer show them around players.

This is Open Roleplay's fork of [VS-RP/SampSharp.OpenMp.Streamer](https://github.com/VS-RP/SampSharp.OpenMp.Streamer). It's the SampSharp 1.x successor of [OpenRP.Streamer](https://github.com/OpenRoleplay/OpenRP.Streamer), which only works on SampSharp 0.x.

## What this fork adds

| | VS-RP's library | This fork |
|---|---|---|
| Streamer items | Plain C# objects | **ECS components**, each on its own entity |
| Owner | None: you destroy items yourself | `parent:` on every `Create…`: the item is destroyed with its owner, and found with `GetComponentInChildren<…>()` |
| Events | Throwaway wrapper objects | The real component: `OnPlayerEnterDynamicCP(Player player, DynamicCheckpoint checkpoint)` |
| Per-player settings | None | `SetVisibleItems`, `SetRadiusMultiplier`, `Update` and their getters |

**Why components:** SampSharp 1.x is ECS only, and everything else in the world (`Player`, `Vehicle`, `GlobalObject`) is a component. Items that belong to an entity, like a door's label, then work the same way.

## Setup

These go in the server's `components/` folder:

| File | From |
|---|---|
| `streamer.dll` / `.so` | [KoertLichtendonk/omp-streamer-component](https://github.com/KoertLichtendonk/omp-streamer-component), the x64 streamer with the per-player methods |
| `OpenRP.OpenMp.Streamer.Native.dll` / `.so` | built from `native/` in this repository |
| `SampSharp.dll` | SampSharp 1.x |

The per-player methods need that `streamer.dll`. VS-RP's own build doesn't have them, and calling them on it would crash.

If `streamer.dll` isn't loaded, every `Create…` returns an item with `Id = 0` and `IsAlive = false`. Check `IStreamerService.IsAvailable`.

## Use

```csharp
// ConfigureServices
services.AddStreamer();

// Anywhere: a label that belongs to a door, destroyed together with it
DynamicTextLabel label = streamerService.CreateDynamicTextLabel("Front door", Color.White, position, 10f,
    parent: door.Entity);

// Per player: fewer objects on a slow PC
streamerService.SetVisibleItems(StreamerType.Object, 500, player);
streamerService.Update(player, StreamerType.Object);
```

## How it works

```
C# gamemode (IStreamerService, DynamicObject, ...)        this repo, C#
   │  P/Invoke (LibraryImport "OpenRP.OpenMp.Streamer.Native")
OpenRP.OpenMp.Streamer.Native.dll  (C exports)            this repo, native/
   │  C++ virtual calls on IStreamerComponent
streamer.dll  (the x64 streamer)                          KoertLichtendonk/omp-streamer-component
```

**Why a native bridge:** SampSharp 1.x can't call Pawn natives, and C# can't call C++ virtual methods. The bridge turns the streamer's C++ interface into plain C functions.

- **Creating:** the service creates the item in the streamer, then adds its component to a new entity (under `parent`, if given).
- **Destroying:** when the component or its entity is destroyed, the item is destroyed in the streamer.
- **Events:** the bridge calls back into C#. The event system finds the item's component by its streamer id, and raises the event with the player's and the item's entities.

## Events

Handlers are ordinary `[Event]` methods. They can take the component as a parameter.

- **Objects:** `OnDynamicObjectMoved`, `OnPlayerEditDynamicObject`, `OnPlayerSelectDynamicObject`, `OnPlayerShootDynamicObject` (`false` vetoes)
- **Pickups:** `OnPlayerPickUpDynamicPickup`
- **Checkpoints:** `OnPlayerEnterDynamicCP` and `OnPlayerLeaveDynamicCP`
- **Stream in and out:** `OnDynamic…StreamIn` and `…StreamOut`, for objects, pickups, labels, map icons and checkpoints
- **Areas and race checkpoints:** `OnPlayerEnter/LeaveDynamicArea` and `…DynamicRaceCP`, with the streamer id (`int`), because this library can't create them yet

## Telemetry and tuning

From VS-RP's version:
- **Stream time and counts per item type:** `GetPhaseStats` and `ResetPhaseStats`;
- **Less flicker at the stream radius:** `SetHysteresisFactor`;
- **A coarse grid for long stream distances:** `TrySetCoarseCellSize` and `TrySetCoarseCellDistance`.

## Building

```bash
# C# library (uses the SampSharp NuGet packages)
dotnet build OpenRP.OpenMp.Streamer.csproj

# native bridge
cmake -B build -S . -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build
```

CMake takes the open.mp SDK from `../omp-streamer-component/lib/open.mp-sdk` (the streamer fork checked out next to this repo, so both use the same SDK revision), or from a SampSharp checkout. Override with `-DOMP_SDK_DIR=<path>`.

On Windows, run CMake in a Visual Studio x64 developer prompt (`vcvars64.bat`). Keep the build folder on a short path, because MSBuild's file tracker fails on long ones.

## Releases

`.github/workflows/release.yml` publishes:

| What | When | Where |
|---|---|---|
| NuGet package `OpenRP.OpenMp.Streamer` | every push to `main` (as `1.0.0-preview.<year>.<week>.<n>`) and every tag (as the tag's version) | nuget.org, through NuGet trusted publishing |
| `OpenRP.OpenMp.Streamer.Native.dll` and `.so` | every tag | the GitHub release for that tag |

`streamer.dll` comes from the [streamer component fork](https://github.com/KoertLichtendonk/omp-streamer-component)'s own releases.

**Why previews from `main`:** the library depends on SampSharp 1.0 prereleases, so its own versions are prereleases too until a tag says otherwise.

## Known limitations
- **Not yet available:** areas, race checkpoints, per-item toggles and reading back materials. The streamer has them, but its C++ interface doesn't expose them yet.
- **Labels:** their text and colour are kept on the C# side, because the interface can't read them back.

## License

Apache-2.0, see [LICENSE](LICENSE). This fork keeps VS-RP's licence and credits; the streamer plugin itself is licensed separately by its authors.

Based on the work of [VS-RP](https://vs-rp.org).
