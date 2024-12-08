using BatteryControl;

Console.WriteLine("Starting battery simulator");
var pool = new BatteryPool();
pool.SetChargeStrategy(new EqualChargeStrategy());
pool.SetDischargeStrategy(new EqualDischargeStrategy());

var source = new PowerCommandSource();
var logger = new CsvLogger(pool, source);

source.SetCallback(newPowerDemand =>
{
    Console.WriteLine($"New power demand {newPowerDemand}");

    if(newPowerDemand is 0)
        Console.WriteLine("Power demand zero. No action taken.");

    pool.DistributePowerDemand(newPowerDemand);
});

Console.WriteLine("Press enter to terminate");
Console.ReadLine();