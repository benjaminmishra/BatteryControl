using BatteryControl;

Console.WriteLine("Starting battery simulator");

using var cts = new CancellationTokenSource();

var pool = new BatteryPool();
pool.SetChargeStrategy(new EqualChargeStrategy());
pool.SetDischargeStrategy(new EqualDischargeStrategy());

var source = new PowerCommandSource();
var logger = new CsvLogger(pool, source);

source.SetCallback(newPowerDemand =>
{
    if(newPowerDemand is 0)
        Console.WriteLine("Power demand zero. No action taken.");

    Task.Run(()=>pool.DistributePowerDemandAsync(newPowerDemand, cts.Token)).Wait();
});

Console.WriteLine("Press enter to terminate");
Console.ReadLine();

await cts.CancelAsync();