using System;

namespace BatteryControl;

public interface IBattery
{
    Task SetNewPower(int newPower);
    int GetCurrentPower();
    bool IsBusy();
}
