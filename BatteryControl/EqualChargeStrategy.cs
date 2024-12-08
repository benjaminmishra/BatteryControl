namespace BatteryControl;

public class EqualChargeStrategy : IChargeStrategy
{
    public async Task ExecuteAsync(int inputPower, IReadOnlyCollection<Battery> batteries, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var orderedBatteriesList = batteries.OrderBy(battery => battery.GetBatteryPercent());
        var powerToAllocate = inputPower;

        foreach (var battery in orderedBatteriesList)
        {
            if(cancellationToken.IsCancellationRequested)
                break;

            // Nothing to allocate, no point in continuing
            if (powerToAllocate <= 0)
                break;
            
            // Battery already full, skip
            if (battery.GetBatteryPercent() >= 100)
                continue;
            
            // TODO: Store the battery and retry later
            if (battery.IsBusy())
                continue;
            
            var powerRequirementOfBattery = battery.MaxChargePower - battery.GetCurrentPower();
            if(powerRequirementOfBattery <= 0)
                continue;

            var maxAllocatablePower = Math.Clamp(powerToAllocate, 0, powerRequirementOfBattery);
            if(maxAllocatablePower == 0)
                continue;

            await battery.SetNewPower(battery.GetCurrentPower() + maxAllocatablePower);

            powerToAllocate -= maxAllocatablePower;
        }
    }
}