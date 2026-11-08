using System.Numerics;
using OpenRP.OpenMp.Streamer.Interop;

namespace OpenRP.OpenMp.Streamer;

/// <summary>Streamer-managed checkpoint (PAWN-native: CreateDynamicCP).</summary>
public sealed class DynamicCheckpoint : DynamicEntity
{
    internal DynamicCheckpoint(int id, Vector3 position, float size)
    {
        Id = id;
        Position = position;
        Size = size;
    }

    public Vector3 Position { get; }
    public float Size { get; }

    public override bool IsAlive => StreamerInterop.Streamer_Checkpoint_IsValid(Id);
    protected override void DestroyNative() => StreamerInterop.Streamer_Checkpoint_Destroy(Id);
}
