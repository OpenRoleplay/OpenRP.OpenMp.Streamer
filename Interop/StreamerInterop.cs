using System.Runtime.InteropServices;

namespace OpenRP.OpenMp.Streamer.Interop;

/// <summary>
/// P/Invoke bindings to the C exports of OpenRP.OpenMp.Streamer.Native.dll, an open.mp component that
/// forwards them to the streamer through its <c>IStreamerComponent</c> extension (the x64 streamer fork).
///
/// If streamer.dll isn't loaded, every Streamer_* call returns 0 or false; check
/// <see cref="Streamer_IsAvailable"/>.
/// </summary>
internal static partial class StreamerInterop
{
    // Our own open.mp component, OpenRP.OpenMp.Streamer.Native.dll, is loaded by the server from components/.
    // LoadLibrary finds an already-loaded module by its short name, without the extension.
    private const string Lib = "OpenRP.OpenMp.Streamer.Native";

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_IsAvailable();

    // ----- Objects ------------------------------------------------------------------

    [LibraryImport(Lib)]
    internal static partial int Streamer_Object_Create(int modelId,
        float posX, float posY, float posZ,
        float rotX, float rotY, float rotZ,
        int worldId, int interiorId, int playerId,
        float streamDistance, float drawDistance,
        int areaId, int priority);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_Destroy(int objectId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_IsValid(int objectId);

    [LibraryImport(Lib)]
    internal static partial int Streamer_Object_Move(int objectId,
        float targetX, float targetY, float targetZ, float speed,
        float rotX, float rotY, float rotZ);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_Stop(int objectId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_IsMoving(int objectId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static unsafe partial bool Streamer_Object_GetPos(int objectId, float* x, float* y, float* z);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_SetPos(int objectId, float x, float y, float z);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static unsafe partial bool Streamer_Object_GetRot(int objectId, float* x, float* y, float* z);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_SetRot(int objectId, float x, float y, float z);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_AttachToObject(int objectId, int parentId,
        float offX, float offY, float offZ, float rotX, float rotY, float rotZ,
        [MarshalAs(UnmanagedType.I1)] bool syncRotation);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_AttachToPlayer(int objectId, int playerId,
        float offX, float offY, float offZ, float rotX, float rotY, float rotZ);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_AttachToVehicle(int objectId, int vehicleId,
        float offX, float offY, float offZ, float rotX, float rotY, float rotZ);

    [LibraryImport(Lib, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_SetMaterial(int objectId, int materialIndex,
        int modelId, string txdName, string textureName, uint materialColor);

    [LibraryImport(Lib, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static unsafe partial bool Streamer_Object_SetMaterialText(int objectId, int materialIndex,
        byte* text, int materialSize, string fontFace, int fontSize,
        [MarshalAs(UnmanagedType.I1)] bool bold, uint fontColor, uint backColor, int alignment);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Object_Edit(int playerId, int objectId);

    // ----- Pickups ------------------------------------------------------------------

    [LibraryImport(Lib)]
    internal static partial int Streamer_Pickup_Create(int modelId, int type,
        float posX, float posY, float posZ,
        int worldId, int interiorId, int playerId,
        float streamDistance, int areaId, int priority);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Pickup_Destroy(int pickupId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Pickup_IsValid(int pickupId);

    // ----- 3D Text Labels -----------------------------------------------------------

    [LibraryImport(Lib)]
    internal static unsafe partial int Streamer_TextLabel_Create(byte* text, uint color,
        float posX, float posY, float posZ, float drawDistance,
        int attachedPlayer, int attachedVehicle,
        [MarshalAs(UnmanagedType.I1)] bool testLos,
        int worldId, int interiorId, int playerId,
        float streamDistance, int areaId, int priority);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_TextLabel_Destroy(int labelId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static unsafe partial bool Streamer_TextLabel_Update(int labelId, uint color, byte* text);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_TextLabel_IsValid(int labelId);

    // ----- Map Icons ----------------------------------------------------------------

    [LibraryImport(Lib)]
    internal static partial int Streamer_MapIcon_Create(float posX, float posY, float posZ,
        int type, uint color,
        int worldId, int interiorId, int playerId,
        float streamDistance, int style, int areaId, int priority);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_MapIcon_Destroy(int iconId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_MapIcon_IsValid(int iconId);

    // ----- Checkpoints --------------------------------------------------------------

    [LibraryImport(Lib)]
    internal static partial int Streamer_Checkpoint_Create(float posX, float posY, float posZ, float size,
        int worldId, int interiorId, int playerId,
        float streamDistance, int areaId, int priority);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Checkpoint_Destroy(int cpId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Checkpoint_IsValid(int cpId);

    // ----- Actors -------------------------------------------------------------------

    [LibraryImport(Lib)]
    internal static partial int Streamer_Actor_Create(int modelId,
        float posX, float posY, float posZ, float rotation,
        [MarshalAs(UnmanagedType.I1)] bool invulnerable, float health, float streamDistance,
        int worldId, int interiorId, int playerId, int areaId, int priority);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_Destroy(int actorId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_IsValid(int actorId);

    [LibraryImport(Lib, StringMarshalling = StringMarshalling.Utf8)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_ApplyAnimation(int actorId, string animLib, string animName,
        float delta,
        [MarshalAs(UnmanagedType.I1)] bool loop,
        [MarshalAs(UnmanagedType.I1)] bool lockX,
        [MarshalAs(UnmanagedType.I1)] bool lockY,
        [MarshalAs(UnmanagedType.I1)] bool freeze, int timeMs);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_ClearAnimations(int actorId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static unsafe partial bool Streamer_Actor_GetPos(int actorId, float* x, float* y, float* z);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_SetPos(int actorId, float x, float y, float z);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static unsafe partial bool Streamer_Actor_GetFacingAngle(int actorId, float* angle);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_SetFacingAngle(int actorId, float angle);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static unsafe partial bool Streamer_Actor_GetHealth(int actorId, float* health);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_SetHealth(int actorId, float health);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_IsInvulnerable(int actorId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_SetInvulnerable(int actorId,
        [MarshalAs(UnmanagedType.I1)] bool invulnerable);

    [LibraryImport(Lib)]
    internal static partial int Streamer_Actor_GetVirtualWorld(int actorId);

    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_Actor_SetVirtualWorld(int actorId, int worldId);

    // ----- Event callback registration ---------------------------------------------

    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_PickUpDynamicPickup(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_EnterDynamicCP(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_LeaveDynamicCP(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_EnterDynamicRaceCP(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_LeaveDynamicRaceCP(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_EnterDynamicArea(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_LeaveDynamicArea(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicObjectMoved(delegate* unmanaged[Cdecl]<int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicObjectStreamIn(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicObjectStreamOut(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicPickupStreamIn(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicPickupStreamOut(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicTextLabelStreamIn(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicTextLabelStreamOut(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicCheckpointStreamIn(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicCheckpointStreamOut(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicMapIconStreamIn(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_DynamicMapIconStreamOut(delegate* unmanaged[Cdecl]<int, int, void> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_PlayerEditDynamicObject(delegate* unmanaged[Cdecl]<int, int, int, float, float, float, float, float, float, int> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_PlayerSelectDynamicObject(delegate* unmanaged[Cdecl]<int, int, int, float, float, float, int> fn);
    [LibraryImport(Lib)] internal static unsafe partial void Streamer_SetCallback_PlayerShootDynamicObject(delegate* unmanaged[Cdecl]<int, int, int, float, float, float, int> fn);

    // ----- Telemetry (VS:RP fork) --------------------------------------------------
    // Per-phase cumulative timings + stream-in/out counts since last reset.
    // `type` is one of STREAMER_TYPE_* (see StreamerType enum). Returns 0 if
    // streamer.dll isn't loaded.

    [LibraryImport(Lib)] internal static partial ulong Streamer_GetPhaseTimeNs(int type);
    [LibraryImport(Lib)] internal static partial ulong Streamer_GetPhaseAvgUs(int type);
    [LibraryImport(Lib)] internal static partial ulong Streamer_GetPhaseTickCount();
    [LibraryImport(Lib)] internal static partial ulong Streamer_GetPhaseStreamInCount(int type);
    [LibraryImport(Lib)] internal static partial ulong Streamer_GetPhaseStreamOutCount(int type);
    [LibraryImport(Lib)] internal static partial void Streamer_ResetPhaseStats();

    // ----- Anti-flicker hysteresis (VS:RP fork) ----------------------------------
    // Per-type [1.0, 10.0]. Default 1.0 = legacy, no hysteresis.

    [LibraryImport(Lib)] internal static partial float Streamer_GetHysteresisFactor(int type);
    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_SetHysteresisFactor(int type, float value);

    // ----- Two-tier grid (VS:RP fork) --------------------------------------------
    // Items with streamDistance in (cellDistance, coarseCellDistance] bucket into
    // coarser cells. Distance = 0 disables the tier.

    [LibraryImport(Lib)] internal static partial float Streamer_GetCoarseCellSize();
    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_SetCoarseCellSize(float size);
    [LibraryImport(Lib)] internal static partial float Streamer_GetCoarseCellDistance();
    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_SetCoarseCellDistance(float distance);

    // ----- Per-player streaming settings (OpenRP streamer fork) -------------------------

    [LibraryImport(Lib)] internal static partial int Streamer_GetVisibleItems(int type, int playerId);
    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_SetVisibleItems(int type, int items, int playerId);
    [LibraryImport(Lib)] internal static partial float Streamer_GetRadiusMultiplier(int type, int playerId);
    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_SetRadiusMultiplier(int type, float multiplier, int playerId);
    [LibraryImport(Lib)]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool Streamer_UpdatePlayer(int playerId, int type);
}
