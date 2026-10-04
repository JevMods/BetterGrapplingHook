using System;

namespace BetterGrapplingHook.Infrastructure
{
    public interface IGameEvents
    {
        event Action<GrapplingPoint> GrappleLaunched;
        event Action<GrapplingPoint> GrappleTickStarting;
        event Action<GrapplingPoint> GrappleTick;
        event Action<GrapplingPoint> GrapplePullEnded;
        event Action<Player> LocalPlayerFixedUpdating;
        event Action<Player> LocalPlayerFixedUpdate;
        event Action<Player, float> LocalPlayerStatsUpdated;
    }
}
