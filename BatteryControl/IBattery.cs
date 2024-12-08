using System;

namespace BatteryControl;

public interface IBattery
{
    int MaxChargePower { get; init; }
    int MaxDischargePower { get; init; }

    Task SetNewPower(int newPower);
    int GetCurrentPower();
    bool IsBusy();
    int GetBatteryPercent();
}