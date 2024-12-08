using System;

namespace BatteryControl;

public interface IDischargeStrategy
{
    public Task ExecuteAsync(int desiredOutputPower, IReadOnlyCollection<Battery> batteries,
        CancellationToken cancellationToken);
}