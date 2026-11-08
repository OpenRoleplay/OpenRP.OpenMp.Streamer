using System;
using System.Numerics;
using System.Runtime.InteropServices;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;
using SampSharp.OpenMp.Core;
using SampSharp.OpenMp.Core.Api;
using OpenRP.OpenMp.Streamer.Interop;

namespace OpenRP.OpenMp.Streamer;

/// <summary>
/// Turns streamer events into ECS events. Registers UnmanagedCallersOnly methods as callbacks in the
/// bridge, which streamer.dll calls on the open.mp tick (main thread). Each event goes through
/// <c>IEventDispatcher.Invoke(...)</c> to the <c>[Event]</c> methods of the systems, with the player's entity
/// and the item's entity, so handlers can take <c>Player</c> and the item's component as parameters.
///
/// The event names are the same as in the 0.x streamer (OnPlayerPickUpDynamicPickup and so on). Events of
/// items this library didn't create (no component) get an empty entity.
/// </summary>
internal sealed class StreamerEventSystem : ISystem
{
    private static IEventDispatcher? _dispatcher;
    private static IOmpEntityProvider? _entityProvider;
    private static bool _registered;
    private static readonly object _sync = new();

    public StreamerEventSystem(IEventDispatcher dispatcher, IOmpEntityProvider entityProvider,
        SampSharpEnvironment environment)
    {
        lock (_sync)
        {
            _dispatcher = dispatcher;
            _entityProvider = entityProvider;
            SampSharpEnvironmentAccessor.Bind(environment);
            if (_registered) return;
            RegisterCallbacks();
            _registered = true;
        }
    }

    private static unsafe void RegisterCallbacks()
    {
        StreamerInterop.Streamer_SetCallback_PickUpDynamicPickup(&OnPickUp);
        StreamerInterop.Streamer_SetCallback_EnterDynamicCP(&OnEnterCp);
        StreamerInterop.Streamer_SetCallback_LeaveDynamicCP(&OnLeaveCp);
        StreamerInterop.Streamer_SetCallback_EnterDynamicRaceCP(&OnEnterRaceCp);
        StreamerInterop.Streamer_SetCallback_LeaveDynamicRaceCP(&OnLeaveRaceCp);
        StreamerInterop.Streamer_SetCallback_EnterDynamicArea(&OnEnterArea);
        StreamerInterop.Streamer_SetCallback_LeaveDynamicArea(&OnLeaveArea);
        StreamerInterop.Streamer_SetCallback_DynamicObjectMoved(&OnObjectMoved);
        StreamerInterop.Streamer_SetCallback_DynamicObjectStreamIn(&OnObjStreamIn);
        StreamerInterop.Streamer_SetCallback_DynamicObjectStreamOut(&OnObjStreamOut);
        StreamerInterop.Streamer_SetCallback_DynamicPickupStreamIn(&OnPkpStreamIn);
        StreamerInterop.Streamer_SetCallback_DynamicPickupStreamOut(&OnPkpStreamOut);
        StreamerInterop.Streamer_SetCallback_DynamicTextLabelStreamIn(&OnLblStreamIn);
        StreamerInterop.Streamer_SetCallback_DynamicTextLabelStreamOut(&OnLblStreamOut);
        StreamerInterop.Streamer_SetCallback_DynamicCheckpointStreamIn(&OnCpStreamIn);
        StreamerInterop.Streamer_SetCallback_DynamicCheckpointStreamOut(&OnCpStreamOut);
        StreamerInterop.Streamer_SetCallback_DynamicMapIconStreamIn(&OnIconStreamIn);
        StreamerInterop.Streamer_SetCallback_DynamicMapIconStreamOut(&OnIconStreamOut);
        StreamerInterop.Streamer_SetCallback_PlayerEditDynamicObject(&OnEditObject);
        StreamerInterop.Streamer_SetCallback_PlayerSelectDynamicObject(&OnSelectObject);
        StreamerInterop.Streamer_SetCallback_PlayerShootDynamicObject(&OnShootObject);
    }

    private static EntityId PlayerEntity(int playerId)
    {
        if (_entityProvider is null) return default;
        try
        {
            var pool = SampSharpEnvironmentAccessor.TryGetPlayerPool();
            if (pool is null) return default;
            var player = pool.Value.Get(playerId);
            if (!player.HasValue) return default;
            return _entityProvider.GetEntity(player);
        }
        catch { return default; }
    }

    // The entity of the item's component, or an empty entity when this library doesn't know the item.
    private static EntityId ItemEntity<T>(int id) where T : DynamicEntity => DynamicEntity.FindByStreamerId<T>(id)?.Entity ?? default;

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnPickUp(int p, int id) =>
        _dispatcher?.Invoke("OnPlayerPickUpDynamicPickup", PlayerEntity(p), ItemEntity<DynamicPickup>(id));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnEnterCp(int p, int id) =>
        _dispatcher?.Invoke("OnPlayerEnterDynamicCP", PlayerEntity(p), ItemEntity<DynamicCheckpoint>(id));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnLeaveCp(int p, int id) =>
        _dispatcher?.Invoke("OnPlayerLeaveDynamicCP", PlayerEntity(p), ItemEntity<DynamicCheckpoint>(id));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnEnterRaceCp(int p, int id) =>
        _dispatcher?.Invoke("OnPlayerEnterDynamicRaceCP", PlayerEntity(p), id);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnLeaveRaceCp(int p, int id) =>
        _dispatcher?.Invoke("OnPlayerLeaveDynamicRaceCP", PlayerEntity(p), id);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnEnterArea(int p, int id) =>
        _dispatcher?.Invoke("OnPlayerEnterDynamicArea", PlayerEntity(p), id);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnLeaveArea(int p, int id) =>
        _dispatcher?.Invoke("OnPlayerLeaveDynamicArea", PlayerEntity(p), id);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnObjectMoved(int id) =>
        _dispatcher?.Invoke("OnDynamicObjectMoved", ItemEntity<DynamicObject>(id));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnObjStreamIn(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicObjectStreamIn", ItemEntity<DynamicObject>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnObjStreamOut(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicObjectStreamOut", ItemEntity<DynamicObject>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnPkpStreamIn(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicPickupStreamIn", ItemEntity<DynamicPickup>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnPkpStreamOut(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicPickupStreamOut", ItemEntity<DynamicPickup>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnLblStreamIn(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicTextLabelStreamIn", ItemEntity<DynamicTextLabel>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnLblStreamOut(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicTextLabelStreamOut", ItemEntity<DynamicTextLabel>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnCpStreamIn(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicCheckpointStreamIn", ItemEntity<DynamicCheckpoint>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnCpStreamOut(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicCheckpointStreamOut", ItemEntity<DynamicCheckpoint>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnIconStreamIn(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicMapIconStreamIn", ItemEntity<DynamicMapIcon>(id), PlayerEntity(f));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static void OnIconStreamOut(int id, int f) =>
        _dispatcher?.Invoke("OnDynamicMapIconStreamOut", ItemEntity<DynamicMapIcon>(id), PlayerEntity(f));

    /// <summary>Return: 0=Continue, 1=Consume (stop propagation), 2=Veto.</summary>
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static int OnEditObject(int p, int o, int r, float x, float y, float z,
        float rx, float ry, float rz)
    {
        var res = _dispatcher?.InvokeAs<object?>("OnPlayerEditDynamicObject", null,
            PlayerEntity(p), ItemEntity<DynamicObject>(o), (EditObjectResponse)r,
            new Vector3(x, y, z), new Vector3(rx, ry, rz));
        return res is true ? 1 : 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static int OnSelectObject(int p, int o, int m, float x, float y, float z)
    {
        var res = _dispatcher?.InvokeAs<object?>("OnPlayerSelectDynamicObject", null,
            PlayerEntity(p), ItemEntity<DynamicObject>(o), m, new Vector3(x, y, z));
        return res is true ? 1 : 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static int OnShootObject(int p, int w, int o, float x, float y, float z)
    {
        var res = _dispatcher?.InvokeAs<object?>("OnPlayerShootDynamicObject", null,
            PlayerEntity(p), w, ItemEntity<DynamicObject>(o), new Vector3(x, y, z));
        return res is false ? 2 /* veto */ : 0 /* continue */;
    }
}

/// <summary>Static holder of the player pool: UnmanagedCallersOnly methods have no <c>this</c>.</summary>
internal static class SampSharpEnvironmentAccessor
{
    private static IPlayerPool? _pool;
    public static void Bind(SampSharpEnvironment env) => _pool = env?.Core.GetPlayers();
    public static IPlayerPool? TryGetPlayerPool() => _pool;
}
