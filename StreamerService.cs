using System.Numerics;
using SampSharp.Entities.SAMP;
using SampSharp.OpenMp.Core.Std;
using SampSharp.Streamer.Entities.Interop;

namespace SampSharp.Streamer.Entities;

/// <summary>
/// <see cref="IStreamerService"/> implementation. Forwards every call into streamer.dll
/// through the C-exports defined in SampSharp.dll (see <see cref="StreamerInterop"/>).
/// </summary>
public sealed class StreamerService : IStreamerService
{
    // 0xFFFF (INVALID_PLAYER_ID / INVALID_VEHICLE_ID) belongs to the OUTGOING
    // wire protocol; passing it into the server-side API is wrong because
    // streamer.dll's addToContainer<bitset<MAX_PLAYERS=1000>>(value) calls
    // .reset() whenever value >= MAX_PLAYERS — the entity ends up visible to
    // nobody. Use -1 instead; streamer treats that as .set() (visible to all).
    private const int BroadcastId = -1;

    // The opposite holds for a text label's ATTACHED player/vehicle: the streamer compares those against
    // INVALID_PLAYER_ID / INVALID_VEHICLE_ID (0xFFFF), so -1 reads as "attached to someone". An attached label is
    // filtered by its attachment's worlds — empty, i.e. every world — so the label's own virtual world was ignored
    // and labels in identical interiors of different worlds showed through each other.
    private const int NotAttachedId = 0xFFFF;

    public bool IsAvailable => StreamerInterop.Streamer_IsAvailable();

    public DynamicObject CreateDynamicObject(int modelId, Vector3 position, Vector3 rotation,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 300f, float drawDistance = 0f,
        int areaId = -1, int priority = 0)
    {
        int id = StreamerInterop.Streamer_Object_Create(modelId,
            position.X, position.Y, position.Z,
            rotation.X, rotation.Y, rotation.Z,
            virtualWorld, interior, PlayerToId(player),
            streamDistance, drawDistance, areaId, priority);
        return new DynamicObject(id, modelId, position, rotation);
    }

    public DynamicPickup CreateDynamicPickup(int modelId, PickupType type, Vector3 position,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, int areaId = -1, int priority = 0)
    {
        int id = StreamerInterop.Streamer_Pickup_Create(modelId, (int)type,
            position.X, position.Y, position.Z,
            virtualWorld, interior, PlayerToId(player),
            streamDistance, areaId, priority);
        return new DynamicPickup(id, modelId, type, position);
    }

    public unsafe DynamicTextLabel CreateDynamicTextLabel(string text, Color color, Vector3 position,
        float drawDistance,
        Player? attachedPlayer = null, Vehicle? attachedVehicle = null,
        bool testLos = false,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, int areaId = -1, int priority = 0)
    {
        uint rgba = ToRgba(color);
        var bytes = EncodeNullTerminated(text ?? string.Empty);
        int id;
        fixed (byte* ptr = bytes)
        {
            id = StreamerInterop.Streamer_TextLabel_Create(ptr, rgba,
                position.X, position.Y, position.Z, drawDistance,
                AttachedId(attachedPlayer), AttachedId(attachedVehicle),
                testLos,
                virtualWorld, interior, PlayerToId(player),
                streamDistance, areaId, priority);
        }

        return new DynamicTextLabel(id, text ?? string.Empty, color, position, drawDistance);
    }

    public DynamicMapIcon CreateDynamicMapIcon(Vector3 position, MapIcon type, Color color,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, MapIconType style = MapIconType.Local,
        int areaId = -1, int priority = 0)
    {
        uint rgba = ToRgba(color);
        int id = StreamerInterop.Streamer_MapIcon_Create(
            position.X, position.Y, position.Z, (int)type, rgba,
            virtualWorld, interior, PlayerToId(player),
            streamDistance, (int)style, areaId, priority);
        return new DynamicMapIcon(id, position, type, color);
    }

    public DynamicCheckpoint CreateDynamicCheckpoint(Vector3 position, float size,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, int areaId = -1, int priority = 0)
    {
        int id = StreamerInterop.Streamer_Checkpoint_Create(
            position.X, position.Y, position.Z, size,
            virtualWorld, interior, PlayerToId(player),
            streamDistance, areaId, priority);
        return new DynamicCheckpoint(id, position, size);
    }

    public DynamicActor CreateDynamicActor(int modelId, Vector3 position, float rotation,
        bool invulnerable = true, float health = 100f,
        float streamDistance = 300f,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        int areaId = -1, int priority = 0)
    {
        int id = StreamerInterop.Streamer_Actor_Create(modelId,
            position.X, position.Y, position.Z, rotation,
            invulnerable, health, streamDistance,
            virtualWorld, interior, PlayerToId(player),
            areaId, priority);
        return new DynamicActor(id, modelId, position, rotation);
    }

    // ---- Telemetry (VS:RP fork) ---------------------------------------------------

    public StreamerPhaseStats GetPhaseStats(StreamerType type)
    {
        int t = (int)type;
        return new StreamerPhaseStats(
            Type: type,
            CumulativeTimeNs: StreamerInterop.Streamer_GetPhaseTimeNs(t),
            AverageMicrosecondsPerPlayerTick: StreamerInterop.Streamer_GetPhaseAvgUs(t),
            StreamInCount: StreamerInterop.Streamer_GetPhaseStreamInCount(t),
            StreamOutCount: StreamerInterop.Streamer_GetPhaseStreamOutCount(t));
    }

    public ulong GetPhaseTickCount() => StreamerInterop.Streamer_GetPhaseTickCount();

    public void ResetPhaseStats() => StreamerInterop.Streamer_ResetPhaseStats();

    // ---- Hysteresis (VS:RP fork) --------------------------------------------------

    public float GetHysteresisFactor(StreamerType type) =>
        StreamerInterop.Streamer_GetHysteresisFactor((int)type);

    public bool SetHysteresisFactor(StreamerType type, float factor) =>
        StreamerInterop.Streamer_SetHysteresisFactor((int)type, factor);

    // ---- Two-tier grid (VS:RP fork) ----------------------------------------------

    public float CoarseCellSize => StreamerInterop.Streamer_GetCoarseCellSize();
    public bool TrySetCoarseCellSize(float size) => StreamerInterop.Streamer_SetCoarseCellSize(size);
    public float CoarseCellDistance => StreamerInterop.Streamer_GetCoarseCellDistance();
    public bool TrySetCoarseCellDistance(float distance) => StreamerInterop.Streamer_SetCoarseCellDistance(distance);

    private static int PlayerToId(Player? player) => player is { IsComponentAlive: true } ? player.Id : BroadcastId;

    private static int VehicleToId(Vehicle? vehicle) =>
        vehicle is { IsComponentAlive: true } ? vehicle.Id : BroadcastId;

    private static int AttachedId(Player? player) => player is { IsComponentAlive: true } ? player.Id : NotAttachedId;

    private static int AttachedId(Vehicle? vehicle) =>
        vehicle is { IsComponentAlive: true } ? vehicle.Id : NotAttachedId;

    private static uint ToRgba(Color c) => ((uint)c.R << 24) | ((uint)c.G << 16) | ((uint)c.B << 8) | c.A;

    /// <summary>
    /// Encodes a string via the current <see cref="StringViewMarshaller.Encoding"/>
    /// and appends a NUL terminator. Used for strings that go DIRECTLY to the client
    /// through streamer.dll — those require the client encodingю
    /// </summary>
    private static byte[] EncodeNullTerminated(string s)
    {
        var enc = StringViewMarshaller.Encoding;
        var byteCount = enc.GetByteCount(s);
        var buffer = new byte[byteCount + 1];
        enc.GetBytes(s, buffer);
        buffer[byteCount] = 0;
        return buffer;
    }
}