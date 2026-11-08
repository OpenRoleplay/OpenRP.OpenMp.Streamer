using System.Numerics;
using SampSharp.Entities;
using SampSharp.Entities.SAMP;

namespace OpenRP.OpenMp.Streamer;

/// <summary>
/// Creates streamer items and changes streamer settings. Every item is an ECS component on its own
/// entity; pass <c>parent</c> to destroy it together with its owner.
///
/// If streamer.dll isn't loaded, items come back with <c>Id = 0</c> and <c>IsAlive = false</c>;
/// check <see cref="IsAvailable"/>.
/// </summary>
public interface IStreamerService
{
    /// <summary>True when streamer.dll is loaded and the bridge found its streamer interface.</summary>
    bool IsAvailable { get; }

    // Objects
    DynamicObject CreateDynamicObject(int modelId, Vector3 position, Vector3 rotation,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 300f, float drawDistance = 0f,
        int areaId = -1, int priority = 0, EntityId parent = default);

    // Pickups
    DynamicPickup CreateDynamicPickup(int modelId, PickupType type, Vector3 position,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, int areaId = -1, int priority = 0, EntityId parent = default);

    // 3D Text Labels
    DynamicTextLabel CreateDynamicTextLabel(string text, Color color, Vector3 position,
        float drawDistance,
        Player? attachedPlayer = null, Vehicle? attachedVehicle = null,
        bool testLos = false,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, int areaId = -1, int priority = 0, EntityId parent = default);

    // Map Icons
    DynamicMapIcon CreateDynamicMapIcon(Vector3 position, MapIcon type, Color color,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, MapIconType style = MapIconType.Local,
        int areaId = -1, int priority = 0, EntityId parent = default);

    // Checkpoints
    DynamicCheckpoint CreateDynamicCheckpoint(Vector3 position, float size,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        float streamDistance = 200f, int areaId = -1, int priority = 0, EntityId parent = default);

    // Actors
    DynamicActor CreateDynamicActor(int modelId, Vector3 position, float rotation,
        bool invulnerable = true, float health = 100f,
        float streamDistance = 300f,
        int virtualWorld = -1, int interior = -1, Player? player = null,
        int areaId = -1, int priority = 0, EntityId parent = default);

    // ========================================================================
    // Telemetry (VS:RP fork) — cumulative since last ResetPhaseStats().
    // Call ResetPhaseStats() every ~1 minute for rolling-window stats.
    // ========================================================================

    /// <summary>Snapshot of timing + stream counts for one streamer type.</summary>
    StreamerPhaseStats GetPhaseStats(StreamerType type);

    /// <summary>Total per-player discovery ticks recorded since the last reset.</summary>
    ulong GetPhaseTickCount();

    /// <summary>Zero every telemetry counter.</summary>
    void ResetPhaseStats();

    // ========================================================================
    // Anti-flicker hysteresis (VS:RP fork). When a player sits on the boundary
    // of an item's stream distance, items ping-pong in/out every tick. With
    // factor > 1.0 the stream-out check is delayed until the player is
    // factor × streamDistance away. Applies to Object / MapIcon / TextLabel;
    // other types ignore the setting.
    // ========================================================================

    float GetHysteresisFactor(StreamerType type);
    /// <summary>Valid range [1.0, 10.0]. Returns false on invalid input.</summary>
    bool SetHysteresisFactor(StreamerType type, float factor);

    // ========================================================================
    // Two-tier grid (VS:RP fork). Items with streamDistance between
    // cellDistance and CoarseCellDistance bucket into larger coarse cells
    // instead of the O(N) globalCell catch-all. CoarseCellDistance = 0
    // disables the tier. Setters rebuild the grid synchronously.
    // ========================================================================

    float CoarseCellSize { get; }
    bool TrySetCoarseCellSize(float size);
    float CoarseCellDistance { get; }
    bool TrySetCoarseCellDistance(float distance);

    // ========================================================================
    // Per-player streaming settings. Needs the OpenRP streamer fork
    // (KoertLichtendonk/omp-streamer-component). A null player means every player.
    // ========================================================================

    /// <summary>How many items of this type a player sees at most.</summary>
    int GetVisibleItems(StreamerType type, Player? player = null);
    /// <summary>Sets how many items of this type a player sees at most.</summary>
    bool SetVisibleItems(StreamerType type, int items, Player? player = null);
    /// <summary>The factor the stream distance of this type is multiplied with for a player.</summary>
    float GetRadiusMultiplier(StreamerType type, Player? player = null);
    /// <summary>Multiplies the stream distance of this type for a player, for example 2 for twice as far.</summary>
    bool SetRadiusMultiplier(StreamerType type, float multiplier, Player? player = null);
    /// <summary>Streams the player's surroundings right away, for one type or (null) all types.</summary>
    bool Update(Player player, StreamerType? type = null);
}
