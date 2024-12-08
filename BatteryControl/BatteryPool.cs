namespace BatteryControl;

/// <summary>
/// A collection of batteries to be controlled.
/// </summary>
public class BatteryPool
{
    private readonly List<Battery> _pool = [];
    private IDischargeStrategy? _dischargeStrategy;
    private IChargeStrategy? _chargeStrategy;

    public BatteryPool()
    {
        for (int i = 0; i < Random.Shared.NextInt64(10, 15); i++)
        {
            _pool.Add(new Battery());
        }

        _ = RunPowerMonitor();
    }
    
    /// <summary>
    /// Sets the strategy used to discharge the pool and get the desired output
    /// </summary>
    /// <param name="dischargeStrategy">Instance represeting the strategy to discharge batteries in the pool</param>
    public void SetDischargeStrategy(IDischargeStrategy dischargeStrategy)
    {
        _dischargeStrategy = dischargeStrategy;
    }
    
    /// <summary>
    /// Sets the strategy used to charge the batteries in the pool when deriving power from the grid
    /// </summary>
    /// <param name="chargeStrategy">Instance represendting the strategy to charge batteries in the pool</param>
    public void SetChargeStrategy(IChargeStrategy chargeStrategy)
    {
        _chargeStrategy = chargeStrategy;
    }

    /// <summary>
    /// Get a list of all the connected batteries 
    /// </summary>
    /// <returns>A list of batteries</returns>
    public IList<Battery> GetConnectedBatteries()
    {
        return _pool.AsReadOnly();
    }

    public void DistributePowerDemand(int newPowerDemand)
    {
        // Assumption:
        // If positive the grid has excess power, so we charge the batteries
        // If negetive the grid has power demand, so we discharge the batteries

        if(newPowerDemand > 0)
        {
            if(_chargeStrategy is null)
                throw new InvalidOperationException("No charge strategy set");

            _chargeStrategy.Execute(newPowerDemand,_pool);
        }
        else if(newPowerDemand < 0)
        {
            if(_dischargeStrategy is null)
                throw new InvalidOperationException("No discharge strategy set");
            
            _dischargeStrategy.Execute(newPowerDemand,_pool);
        }
    }

    private async Task RunPowerMonitor()
    {
        while (true)
        {
            await Task.Delay(1000);
            
            var currentPower = _pool.Sum(battery => battery.GetCurrentPower());
            Console.WriteLine($"Current set power: {currentPower}");
        }
    }
}