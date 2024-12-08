using System;

namespace BatteryControl;

public interface IDischargeStrategy
{
    public void Execute(int desiredOutputPower, IReadOnlyCollection<Battery> batteries);
}