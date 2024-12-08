using Moq;

namespace BatteryControl.Tests;

[Trait("Category", "Unit")]
public class DefaultChargeStrategyTests
{
    [Fact]
    public async Task ExecuteAsync_AllocatesPowerToBatteriesInOrderOfCharge()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(50, 50,false, maxChargePower: 100),
                TestHelpers.CreateBatteryMock(20, 20,false, maxChargePower: 100),
                TestHelpers.CreateBatteryMock(80, 80, false, maxChargePower: 100)
            };

        var strategy = new DefaultChargeStrategy();
        var inputPower = 120;
        var cancellationToken = CancellationToken.None;

        await strategy.ExecuteAsync(inputPower, batteries.Select(b => b.Object).ToList(), cancellationToken);

        batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
        batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
        batteries[2].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_SkipsFullBatteries()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(100, 10, false, maxChargePower: 100),
                TestHelpers.CreateBatteryMock(90, 10, false, maxChargePower: 100)
            };

        var strategy = new DefaultChargeStrategy();
        var inputPower = 50;
        var cancellationToken = CancellationToken.None;

        await strategy.ExecuteAsync(inputPower, batteries.Select(b => b.Object).ToList(), cancellationToken);

        batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
        batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesBusyBatteries()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(50, 10, true, maxChargePower: 100),
                TestHelpers.CreateBatteryMock(20, 10, false, maxChargePower: 100)
            };

        var strategy = new DefaultChargeStrategy();
        var inputPower = 50;
        var cancellationToken = CancellationToken.None;

        await strategy.ExecuteAsync(inputPower, batteries.Select(b => b.Object).ToList(), cancellationToken);

        batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
        batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
    }
}

