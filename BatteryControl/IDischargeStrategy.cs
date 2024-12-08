using System;

namespace BatteryControl;

public interface IDischargeStrategy
{
    public Task ExecuteAsync(int desiredOutputPower, IReadOnlyCollection<IBattery> batteries, CancellationToken cancellationToken);
}