using System;

namespace BetterGrapplingHook.Infrastructure
{
    public sealed class GameEvents : IGameEvents
    {
        internal static GameEvents Instance { get; set; }

        public event Action<GrapplingPoint> GrappleLaunched;
        public event Action<GrapplingPoint> GrappleTickStarting;
        public event Action<GrapplingPoint> GrappleTick;
        public event Action<GrapplingPoint> GrapplePullEnded;
        public event Action<Player> LocalPlayerFixedUpdating;
        public event Action<Player> LocalPlayerFixedUpdate;
        public event Action<Player, float> LocalPlayerStatsUpdated;

        internal void RaiseGrappleLaunched(GrapplingPoint point)
        {
            GrappleLaunched?.Invoke(point);
        }

        internal void RaiseGrappleTickStarting(GrapplingPoint point)
        {
            GrappleTickStarting?.Invoke(point);
        }

        internal void RaiseGrappleTick(GrapplingPoint point)
        {
            GrappleTick?.Invoke(point);
        }

        internal void RaiseGrapplePullEnded(GrapplingPoint point)
        {
            GrapplePullEnded?.Invoke(point);
        }

        internal void RaiseLocalPlayerFixedUpdating(Player player)
        {
            LocalPlayerFixedUpdating?.Invoke(player);
        }

        internal void RaiseLocalPlayerFixedUpdate(Player player)
        {
            LocalPlayerFixedUpdate?.Invoke(player);
        }

        internal void RaiseLocalPlayerStatsUpdated(Player player, float staminaBefore)
        {
            LocalPlayerStatsUpdated?.Invoke(player, staminaBefore);
        }
    }
}
