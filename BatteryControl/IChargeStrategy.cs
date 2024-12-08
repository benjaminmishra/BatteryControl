using System;

namespace BatteryControl;

public interface IChargeStrategy
{
    public void Execute(int inputPower, IReadOnlyCollection<Battery> batteries);
}