using Microsoft.Extensions.DependencyInjection;
using SampSharp.Entities;
using SampSharp.OpenMp.Core;

namespace OpenRP.OpenMp.Streamer;

public static class StreamerEcsExtensions
{
    /// <summary>
    /// Kept for symmetry with the 0.x streamer: the events are already enabled by
    /// <see cref="StreamerEventSystem"/>, which <see cref="AddStreamer"/> registers.
    /// </summary>
    public static IEcsBuilder EnableStreamerEvents(this IEcsBuilder builder) => builder;

    /// <summary>
    /// Registers <see cref="IStreamerService"/> and the streamer event system. Call it from ConfigureServices.
    /// </summary>
    public static IServiceCollection AddStreamer(this IServiceCollection services)
    {
        services.AddSingleton<IStreamerService, StreamerService>();
        services.AddSystem<StreamerEventSystem>();
        return services;
    }
}
