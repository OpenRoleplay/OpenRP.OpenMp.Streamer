using System.Numerics;
using OpenRP.OpenMp.Streamer.Interop;

namespace OpenRP.OpenMp.Streamer;

/// <summary>Streamer-managed actor (PAWN-native: CreateDynamicActor).</summary>
public sealed class DynamicActor : DynamicEntity
{
    internal DynamicActor(int id, int modelId, Vector3 position, float rotation)
    {
        Id = id;
        ModelId = modelId;
        Position = position;
        Rotation = rotation;
    }

    public int ModelId { get; }
    public Vector3 Position { get; private set; }
    public float Rotation { get; private set; }

    public override bool IsAlive => StreamerInterop.Streamer_Actor_IsValid(Id);
    protected override void DestroyNative() => StreamerInterop.Streamer_Actor_Destroy(Id);

    public bool SetPosition(Vector3 pos)
    {
        if (!StreamerInterop.Streamer_Actor_SetPos(Id, pos.X, pos.Y, pos.Z)) return false;
        Position = pos;
        return true;
    }

    public bool SetFacingAngle(float angle)
    {
        if (!StreamerInterop.Streamer_Actor_SetFacingAngle(Id, angle)) return false;
        Rotation = angle;
        return true;
    }

    public float Health
    {
        get
        {
            unsafe
            {
                float h = 0f;
                StreamerInterop.Streamer_Actor_GetHealth(Id, &h);
                return h;
            }
        }
        set => StreamerInterop.Streamer_Actor_SetHealth(Id, value);
    }

    public bool Invulnerable
    {
        get => StreamerInterop.Streamer_Actor_IsInvulnerable(Id);
        set => StreamerInterop.Streamer_Actor_SetInvulnerable(Id, value);
    }

    public int VirtualWorld
    {
        get => StreamerInterop.Streamer_Actor_GetVirtualWorld(Id);
        set => StreamerInterop.Streamer_Actor_SetVirtualWorld(Id, value);
    }

    public bool ApplyAnimation(string animLib, string animName, float delta,
        bool loop, bool lockX, bool lockY, bool freeze, int timeMs)
        => StreamerInterop.Streamer_Actor_ApplyAnimation(Id, animLib, animName, delta,
            loop, lockX, lockY, freeze, timeMs);

    public bool ClearAnimations() => StreamerInterop.Streamer_Actor_ClearAnimations(Id);
}
