namespace OpenRP.OpenMp.Streamer;

/// <summary>
/// Identifies an item kind the streamer tracks. Mirrors the C side
/// <c>STREAMER_TYPE_*</c> constants. Order matters — values are used as indices
/// into per-type arrays in the native plugin.
/// </summary>
public enum StreamerType
{
    Object = 0,
    Pickup = 1,
    Checkpoint = 2,
    RaceCheckpoint = 3,
    MapIcon = 4,
    TextLabel = 5,
    Area = 6,
    Actor = 7,
}

/// <summary>
/// Snapshot of cumulative streamer work for one item type since the last
/// <see cref="IStreamerService.ResetPhaseStats"/>. Time is wall-clock, not CPU.
/// Player-tick = one pass of per-player discovery for one player; total work
/// per real tick is the sum of this across all connected players.
/// </summary>
/// <param name="Type">Which streamer type these figures describe.</param>
/// <param name="CumulativeTimeNs">Total nanoseconds spent in the process* phase for this type.</param>
/// <param name="AverageMicrosecondsPerPlayerTick">Cumulative / tickCount / 1000. Handy as a single-value health metric.</param>
/// <param name="StreamInCount">Number of client-create calls issued for items of this type since the last reset.</param>
/// <param name="StreamOutCount">Number of client-destroy calls issued for items of this type since the last reset.</param>
public readonly record struct StreamerPhaseStats(
    StreamerType Type,
    ulong CumulativeTimeNs,
    ulong AverageMicrosecondsPerPlayerTick,
    ulong StreamInCount,
    ulong StreamOutCount);
