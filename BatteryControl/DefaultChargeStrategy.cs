namespace BatteryControl;

public class DefaultChargeStrategy : IChargeStrategy
{
    public async Task ExecuteAsync(
        int inputPower,
        IReadOnlyCollection<IBattery> batteries,
        CancellationToken cancellationToken)
    {
        // Nothing to allocate, no point in continuing
        if (inputPower <= 0)
            return;

        if (batteries.Count is 0)
            return;

        var orderedBatteriesList = batteries.OrderBy(battery => battery.GetBatteryPercent());
        var powerToAllocate = inputPower;

        foreach (var battery in orderedBatteriesList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // Battery already full, skip
            if (battery.GetBatteryPercent() >= 100)
                continue;

            // Nothing more to allocate
            if (powerToAllocate <= 0)
                break;

            // TODO: Store the battery and retry later
            if (battery.IsBusy())
                continue;

            var powerRequirementOfBattery = battery.MaxChargePower - battery.GetCurrentPower();
            if (powerRequirementOfBattery <= 0)
                continue;

            var allocatablePower = Math.Clamp(powerToAllocate, 0, powerRequirementOfBattery);
            if (allocatablePower == 0)
                continue;

            await battery.SetNewPower(battery.GetCurrentPower() + allocatablePower);

            powerToAllocate -= allocatablePower;
        }
    }
}