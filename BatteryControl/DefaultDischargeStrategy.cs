namespace BatteryControl;

public class DefaultDischargeStrategy : IDischargeStrategy
{
    public async Task ExecuteAsync(
        int desiredOutputPower,
        IReadOnlyCollection<IBattery> batteries,
        CancellationToken cancellationToken)
    {
        if (desiredOutputPower <= 0)
            return;

        if (batteries.Count is 0)
            return;

        var orderedBatteriesList = batteries.OrderByDescending(battery => battery.GetBatteryPercent());
        var powerToExtract = desiredOutputPower;

        foreach (var battery in orderedBatteriesList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // Nothing more to extract
            if (powerToExtract <= 0)
                break;

            // Battery cannot be discharged further
            if (battery.GetBatteryPercent() <= 0)
                continue;

            // TODO: Store the battery and retry later
            if (battery.IsBusy())
                continue;

            var maxPowerAvaiableToExtract = battery.GetCurrentPower() - battery.MaxDischargePower;
            if (maxPowerAvaiableToExtract <= 0)
                continue;

            var extractablePower = Math.Clamp(powerToExtract, 0, maxPowerAvaiableToExtract);
            if (extractablePower == 0)
                continue;

            // Negate it as we want to discharge
            await battery.SetNewPower((battery.GetCurrentPower() - extractablePower) * -1);

            powerToExtract -= extractablePower;
        }
    }
}