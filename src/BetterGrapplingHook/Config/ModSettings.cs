using BepInEx.Configuration;

namespace BetterGrapplingHook.Config
{
    public sealed class ModSettings : IModSettings
    {
        private readonly ConfigEntry<float> _launchLift;
        private readonly ConfigEntry<float> _liftDuration;
        private readonly ConfigEntry<ArrivalBehavior> _onArrival;

        public ModSettings(ConfigFile file)
        {
            _launchLift = file.Bind(
                "Flight",
                "LaunchLift",
                6f,
                new ConfigDescription(
                    "Extra upward speed at launch, in m/s.",
                    new AcceptableValueRange<float>(0f, 20f)));

            _liftDuration = file.Bind(
                "Flight",
                "LiftDuration",
                0.75f,
                new ConfigDescription(
                    "How long the extra upward speed lasts, in seconds.",
                    new AcceptableValueRange<float>(0.1f, 3f)));

            _onArrival = file.Bind(
                "Flight",
                "OnArrival",
                ArrivalBehavior.Hang,
                "What happens when you reach the hook. "
                    + "Hang = stay attached and hold in place. AutoRetract = release the hook.");
        }

        public float LaunchLift => _launchLift.Value;

        public float LiftDuration => _liftDuration.Value;

        public ArrivalBehavior OnArrival => _onArrival.Value;
    }
}
