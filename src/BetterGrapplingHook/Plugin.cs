using BepInEx;
using BetterGrapplingHook.Config;
using BetterGrapplingHook.Features;
using BetterGrapplingHook.Infrastructure;
using HarmonyLib;

namespace BetterGrapplingHook
{
    [BepInPlugin(PluginGuid, PluginName, BuildInfo.Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "JevMods.BetterGrapplingHook";
        public const string PluginName = "BetterGrapplingHook";

        private AssistedGrappleFeature _feature;
        private Harmony _harmony;

        private void Awake()
        {
            var events = new GameEvents();
            GameEvents.Instance = events;

            _feature = new AssistedGrappleFeature(events, new ModSettings(Config));
            _feature.Enable();

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Logger.LogInfo("Loaded");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            _feature?.Disable();
            GameEvents.Instance = null;
        }
    }
}
