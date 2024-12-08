using System;

namespace BatteryControl;

public interface IChargeStrategy
{
    public Task ExecuteAsync(int inputPower, IReadOnlyCollection<IBattery> batteries, CancellationToken cancellationToken);
}