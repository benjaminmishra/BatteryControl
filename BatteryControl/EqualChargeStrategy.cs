namespace BatteryControl;

public class EqualChargeStrategy : IChargeStrategy
{
    public void Execute(int inputPower, IReadOnlyCollection<Battery> batteries)
    {
        var orderedBatteriesList = batteries.OrderBy(battery => battery.GetBatteryPercent());
        var remainingPowerToAllocate = inputPower;
        //var 

        foreach (var battery in orderedBatteriesList)
        {
            if (remainingPowerToAllocate <= 0)
                break;
            
            //if(battery.IsBusy())
                

            var maxAllocatablePower = Math.Clamp(
                remainingPowerToAllocate,
                -battery.MaxDischargePower,
                battery.MaxChargePower);
            
            
        }

    }
}