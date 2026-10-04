using UnityEngine;

namespace BetterGrapplingHook.Features
{
    public static class GrappleMath
    {
        private const float FloorNormalY = 0.5f;

        public static float HangDepth(float anchorAbovePlayer, float maxDepth, float surfaceNormalY)
        {
            if (surfaceNormalY >= FloorNormalY)
            {
                return 0f;
            }

            return Mathf.Clamp(anchorAbovePlayer, 0f, maxDepth);
        }

        public static float LiftSpeed(float lift, float duration, float elapsed)
        {
            if (duration <= 0f)
            {
                return 0f;
            }

            return lift * Mathf.Clamp01(1f - elapsed / duration);
        }
    }
}
