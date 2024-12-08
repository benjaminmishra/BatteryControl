namespace BatteryControl;

public class EqualDischargeStrategy : IDischargeStrategy
{
    public async Task ExecuteAsync(int desiredOutputPower, IReadOnlyCollection<Battery> batteries,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var orderedBatteriesList = batteries.OrderByDescending(battery => battery.GetBatteryPercent());
        var powerToExtract = desiredOutputPower;

        foreach (var battery in orderedBatteriesList)
        {
            if(cancellationToken.IsCancellationRequested)
                break;

            // Nothing to extract, no point in continuing
            if (powerToExtract <= 0)
                break;
            
            // Battery cannot be discharged further
            if (battery.GetBatteryPercent() <= 0)
                continue;
            
            // TODO: Store the battery and retry later
            if (battery.IsBusy())
                continue;
            
            var powerAvaiableToExtractFromBattery = battery.GetCurrentPower() - battery.MaxDischargePower;
            if(powerAvaiableToExtractFromBattery <= 0)
                continue;

            var maxExtractablePower = Math.Clamp(powerToExtract, 0, powerAvaiableToExtractFromBattery);
            if (maxExtractablePower == 0)
                continue;

            // Negate it as we want to discharge
            await battery.SetNewPower((battery.GetCurrentPower() - maxExtractablePower)*-1);

            powerToExtract -= maxExtractablePower;
        }
    }
}