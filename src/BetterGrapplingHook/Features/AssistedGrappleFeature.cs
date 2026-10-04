using BetterGrapplingHook.Config;
using BetterGrapplingHook.Infrastructure;
using UnityEngine;

namespace BetterGrapplingHook.Features
{
    public sealed class AssistedGrappleFeature
    {
        private const float HangDepth = 3f;
        private const float GroundSettleDistance = 0.6f;
        private const float MaxProbeError = 2f;

        private static readonly RaycastHit[] ProbeHits = new RaycastHit[32];

        private readonly IGameEvents _events;
        private readonly IModSettings _settings;
        private GrapplingPoint _attached;
        private GrapplingPoint _hanging;
        private float _depth;
        private bool _gravityOff;

        public AssistedGrappleFeature(IGameEvents events, IModSettings settings)
        {
            _events = events;
            _settings = settings;
        }

        public void Enable()
        {
            _events.GrappleLaunched += OnLaunched;
            _events.GrappleTickStarting += OnTickStarting;
            _events.GrappleTick += OnTick;
            _events.GrapplePullEnded += OnPullEnded;
            _events.LocalPlayerFixedUpdating += OnPhysicsStepStarting;
            _events.LocalPlayerFixedUpdate += OnPhysicsStep;
            _events.LocalPlayerStatsUpdated += OnStatsUpdated;
        }

        public void Disable()
        {
            _events.GrappleLaunched -= OnLaunched;
            _events.GrappleTickStarting -= OnTickStarting;
            _events.GrappleTick -= OnTick;
            _events.GrapplePullEnded -= OnPullEnded;
            _events.LocalPlayerFixedUpdating -= OnPhysicsStepStarting;
            _events.LocalPlayerFixedUpdate -= OnPhysicsStep;
            _events.LocalPlayerStatsUpdated -= OnStatsUpdated;
        }

        private void OnLaunched(GrapplingPoint point)
        {
            if (!IsConstantVelocity(point))
            {
                return;
            }

            _attached = point;
            _depth = ChooseDepth(point);
            point.m_closeBreakDist = Mathf.Max(point.m_closeBreakDist, _depth + 0.5f);

            var character = point.m_character;
            var toTarget = (Target(point) - character.transform.position).normalized;
            var launch = toTarget * point.m_pullForce + Vector3.up * _settings.LaunchLift;
            character.ForceJump(launch, effects: false);
        }

        private void OnTickStarting(GrapplingPoint point)
        {
            if (!IsConstantVelocity(point))
            {
                return;
            }

            var offset = point.m_character.transform.position - point.transform.position;
            if (offset.magnitude < point.m_lastDist)
            {
                point.m_breakingTime = 0f;
            }
        }

        private void OnTick(GrapplingPoint point)
        {
            if (!IsConstantVelocity(point))
            {
                return;
            }

            var character = point.m_character;
            var toTarget = Target(point) - character.transform.position;
            var velocity = Vector3.zero;
            if (toTarget.sqrMagnitude > 0.01f)
            {
                velocity = toTarget.normalized * point.m_pullForce;
            }

            var lift = GrappleMath.LiftSpeed(
                _settings.LaunchLift, _settings.LiftDuration, point.m_time);
            character.SetVelocity(velocity + Vector3.up * lift);
        }

        private void OnPullEnded(GrapplingPoint point)
        {
            if (!IsConstantVelocity(point))
            {
                return;
            }

            if (_settings.OnArrival == ArrivalBehavior.AutoRetract)
            {
                point.Break(early: true);
                return;
            }

            _hanging = point;
            point.StandUp();
        }

        private void OnPhysicsStepStarting(Player player)
        {
            if (IsHolding())
            {
                player.SetMoveDir(Vector3.zero);
            }
        }

        private void OnPhysicsStep(Player player)
        {
            if (IsHolding())
            {
                HoldInPlace(player);
                return;
            }

            _hanging = null;
            if (_gravityOff)
            {
                player.m_body.useGravity = true;
                _gravityOff = false;
            }
        }

        private void OnStatsUpdated(Player player, float staminaBefore)
        {
            if (!IsAttached())
            {
                _attached = null;
                return;
            }

            if (player.m_stamina > staminaBefore)
            {
                player.m_stamina = staminaBefore;
            }

            if (player.m_stamina <= 0f)
            {
                _attached.Break(early: true);
            }
        }

        private void HoldInPlace(Player player)
        {
            player.SetVelocity(Vector3.zero);

            var nearGround = IsGroundWithin(player.transform.position, GroundSettleDistance);
            player.m_body.useGravity = nearGround;
            _gravityOff = !nearGround;
        }

        private float ChooseDepth(GrapplingPoint point)
        {
            var feet = point.m_character.transform.position;
            var anchor = point.transform.position;

            var normalY = SurfaceNormalY(feet, anchor);
            var depth = GrappleMath.HangDepth(anchor.y - feet.y, HangDepth, normalY ?? 1f);

            var groundY = GroundHeightBelow(anchor, (anchor - feet).normalized);
            if (groundY.HasValue)
            {
                depth = Mathf.Min(depth, Mathf.Max(0f, anchor.y - groundY.Value));
            }

            return depth;
        }

        private Vector3 Target(GrapplingPoint point)
        {
            return point.transform.position + Vector3.down * _depth;
        }

        private bool IsAttached()
        {
            return _attached != null && IsLineVisible(_attached);
        }

        private bool IsHolding()
        {
            return _hanging != null && _hanging.m_secondary && IsLineVisible(_hanging);
        }

        private static bool IsLineVisible(GrapplingPoint point)
        {
            return point.m_lineRenderer != null && point.m_lineRenderer.enabled;
        }

        private static bool IsConstantVelocity(GrapplingPoint point)
        {
            return point.Method == GrapplingPoint.GrapplingMethod.ConstantVelocity;
        }

        private static bool IsEnvironment(RaycastHit hit)
        {
            var isCharacter = hit.collider.GetComponentInParent<Character>() != null;
            var isHook = hit.collider.GetComponentInParent<GrapplingPoint>() != null;
            return !isCharacter && !isHook;
        }

        private static float? GroundHeightBelow(Vector3 anchor, Vector3 toAnchor)
        {
            var origin = anchor - toAnchor * 0.3f;
            var count = Physics.RaycastNonAlloc(
                origin,
                Vector3.down,
                ProbeHits,
                100f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            float? groundY = null;
            var nearest = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var hit = ProbeHits[i];
                if (IsEnvironment(hit) && hit.distance < nearest)
                {
                    nearest = hit.distance;
                    groundY = hit.point.y;
                }
            }

            return groundY;
        }

        private static bool IsGroundWithin(Vector3 feet, float distance)
        {
            const float startHeight = 0.5f;
            var count = Physics.RaycastNonAlloc(
                feet + Vector3.up * startHeight,
                Vector3.down,
                ProbeHits,
                startHeight + distance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            for (var i = 0; i < count; i++)
            {
                if (IsEnvironment(ProbeHits[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static float? SurfaceNormalY(Vector3 feet, Vector3 anchor)
        {
            var origin = feet + Vector3.up;
            var toAnchor = anchor - origin;
            var distance = toAnchor.magnitude;
            if (distance < 1f)
            {
                return null;
            }

            var count = Physics.RaycastNonAlloc(
                origin,
                toAnchor / distance,
                ProbeHits,
                distance + 1f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            float? normalY = null;
            var closest = MaxProbeError;
            for (var i = 0; i < count; i++)
            {
                var hit = ProbeHits[i];
                var error = (hit.point - anchor).magnitude;
                if (IsEnvironment(hit) && error < closest)
                {
                    closest = error;
                    normalY = hit.normal.y;
                }
            }

            return normalY;
        }
    }
}
