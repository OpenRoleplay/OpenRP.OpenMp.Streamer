using System.Numerics;
using SampSharp.Entities.SAMP;
using OpenRP.OpenMp.Streamer.Interop;

namespace OpenRP.OpenMp.Streamer;

/// <summary>Streamer-managed map icon (PAWN-native: CreateDynamicMapIcon).</summary>
public sealed class DynamicMapIcon : DynamicEntity
{
    internal DynamicMapIcon(int id, Vector3 position, MapIcon type, Color color)
    {
        Id = id;
        Position = position;
        Type = type;
        Color = color;
    }

    public Vector3 Position { get; }
    public MapIcon Type { get; }
    public Color Color { get; }

    public override bool IsAlive => StreamerInterop.Streamer_MapIcon_IsValid(Id);
    protected override void DestroyNative() => StreamerInterop.Streamer_MapIcon_Destroy(Id);
}

public enum MapIconType
{
    Local = 0,
    Global = 1,
    LocalCheckpoint = 2,
    GlobalCheckpoint = 3,
}
