using System;
using System.Collections.Concurrent;
using SampSharp.Entities;

namespace OpenRP.OpenMp.Streamer;

/// <summary>
/// Base class of every streamer item (objects, pickups, text labels, map icons, checkpoints, actors).
/// </summary>
/// <remarks>
/// An item is an ECS <see cref="Component"/> on its own entity. That entity can have a parent (pass
/// <c>parent</c> to a <c>Create…</c> method of <see cref="IStreamerService"/>), so the item is destroyed
/// together with its owner, and owners can find their items with <c>GetComponentInChildren</c>.
/// Destroying the component (or its entity) destroys the item in the streamer.
/// </remarks>
public abstract class DynamicEntity : Component
{
    // Streamer events report a streamer id; this finds the component that belongs to it.
    private static readonly ConcurrentDictionary<(Type Type, int Id), DynamicEntity> _itemsByStreamerId = new();

    /// <summary>The item's id inside the streamer (0 when the streamer refused to create it).</summary>
    public int Id { get; protected set; }

    /// <summary>True while the streamer still has this item.</summary>
    public abstract bool IsAlive { get; }

    /// <summary>Same as <see cref="IsAlive"/>.</summary>
    public bool IsValid => IsAlive;

    /// <summary>Removes the item from the streamer. Called when the component is destroyed.</summary>
    protected abstract void DestroyNative();

    /// <summary>The live component of this type for a streamer id, or null.</summary>
    internal static T? FindByStreamerId<T>(int id) where T : DynamicEntity
    {
        return _itemsByStreamerId.TryGetValue((typeof(T), id), out var item) ? (T)item : null;
    }

    protected override void OnInitializeComponent()
    {
        _itemsByStreamerId[(GetType(), Id)] = this;
    }

    protected override void OnDestroyComponent()
    {
        _itemsByStreamerId.TryRemove(new((GetType(), Id), this));
        DestroyNative();
    }

    public override string ToString() => $"{GetType().Name}(Id={Id})";
}
