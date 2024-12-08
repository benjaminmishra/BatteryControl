using Moq;

namespace BatteryControl.Tests;

public static class TestHelpers
{
    public static Mock<IBattery> CreateBatteryMock(
        int batteryPercent,
        int currentPower,
        bool isBusy,
        int? maxChargePower = null,
        int? maxDischargePower = null)
    {
        var batteryMock = new Mock<IBattery>();
        batteryMock.Setup(b => b.GetBatteryPercent()).Returns(batteryPercent);
        batteryMock.Setup(b => b.GetCurrentPower()).Returns(currentPower);
        batteryMock.Setup(b => b.IsBusy()).Returns(isBusy);
        batteryMock.Setup(b => b.SetNewPower(It.IsAny<int>())).Returns(Task.CompletedTask);

        if(maxChargePower is not null)
            batteryMock.SetupGet(b => b.MaxChargePower).Returns(maxChargePower.Value);
            
        if(maxDischargePower is not null)
            batteryMock.Setup(b => b.MaxDischargePower).Returns(maxDischargePower.Value);

        return batteryMock;
    }
}
