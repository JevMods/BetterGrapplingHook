using BetterGrapplingHook.Infrastructure;
using HarmonyLib;

namespace BetterGrapplingHook.Patches
{
    internal static class GrapplingPointPatches
    {
        private static bool IsLocalPlayer(Character character)
        {
            return character != null && character == Player.m_localPlayer;
        }

        private static bool IsLocalHook(GrapplingPoint point)
        {
            if (point == null || !IsLocalPlayer(point.m_character))
            {
                return false;
            }

            var view = point.m_nview;
            var isOwner = view != null && view.IsValid() && view.IsOwner();
            var lineVisible = point.m_lineRenderer != null && point.m_lineRenderer.enabled;
            return isOwner && lineVisible;
        }

        private static bool IsLocalPulling(GrapplingPoint point)
        {
            return IsLocalHook(point) && !point.m_secondary;
        }

        [HarmonyPatch(typeof(GrapplingPoint), nameof(GrapplingPoint.Activate))]
        private static class Activate
        {
            private static void Postfix(GrapplingPoint __instance)
            {
                if (IsLocalPulling(__instance))
                {
                    GameEvents.Instance?.RaiseGrappleLaunched(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(GrapplingPoint), "Update")]
        private static class Update
        {
            private static void Prefix(GrapplingPoint __instance)
            {
                if (IsLocalPulling(__instance))
                {
                    GameEvents.Instance?.RaiseGrappleTickStarting(__instance);
                }
            }

            private static void Postfix(GrapplingPoint __instance)
            {
                if (IsLocalPulling(__instance))
                {
                    GameEvents.Instance?.RaiseGrappleTick(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(GrapplingPoint), "Deactivate")]
        private static class Deactivate
        {
            private static void Postfix(GrapplingPoint __instance)
            {
                if (IsLocalHook(__instance))
                {
                    GameEvents.Instance?.RaiseGrapplePullEnded(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(Character), nameof(Character.CustomFixedUpdate))]
        private static class CharacterFixedUpdate
        {
            private static void Prefix(Character __instance)
            {
                if (IsLocalPlayer(__instance))
                {
                    GameEvents.Instance?.RaiseLocalPlayerFixedUpdating(Player.m_localPlayer);
                }
            }

            private static void Postfix(Character __instance)
            {
                if (IsLocalPlayer(__instance))
                {
                    GameEvents.Instance?.RaiseLocalPlayerFixedUpdate(Player.m_localPlayer);
                }
            }
        }

        [HarmonyPatch(typeof(Player), "UpdateStats", typeof(float))]
        private static class PlayerUpdateStats
        {
            private static void Prefix(Player __instance, out float __state)
            {
                __state = __instance.m_stamina;
            }

            private static void Postfix(Player __instance, float __state)
            {
                if (IsLocalPlayer(__instance))
                {
                    GameEvents.Instance?.RaiseLocalPlayerStatsUpdated(__instance, __state);
                }
            }
        }
    }
}
