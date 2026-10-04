namespace BetterGrapplingHook.Config
{
    public interface IModSettings
    {
        float LaunchLift { get; }
        float LiftDuration { get; }
        ArrivalBehavior OnArrival { get; }
    }
}
