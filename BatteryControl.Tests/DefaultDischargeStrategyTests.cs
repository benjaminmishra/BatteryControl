using Moq;

namespace BatteryControl.Tests;

[Trait("Category", "Unit")]
public class DefaultDischargeStrategyTests
{
    [Fact]
    public async Task ExecuteAsync_DoesNothing_WhenDesiredOutputPowerIsZero()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(50, 100, false, maxDischargePower: 10),
                TestHelpers.CreateBatteryMock(70, 100, false, maxDischargePower: 10)
            };

        var strategy = new DefaultDischargeStrategy();
        await strategy.ExecuteAsync(0, batteries.Select(b => b.Object).ToList(), CancellationToken.None);

        foreach (var batteryMock in batteries)
        {
            batteryMock.Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
        }
    }

    [Fact]
    public async Task ExecuteAsync_DoesNothing_WhenNoBatteriesProvided()
    {
        var strategy = new DefaultDischargeStrategy();
        await strategy.ExecuteAsync(100, new List<IBattery>(), CancellationToken.None);

        // No exception should occur
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteAsync_SkipsBusyBatteries()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(70, 100, true, maxDischargePower: 10), // Busy battery
                TestHelpers.CreateBatteryMock(50, 100, false, maxDischargePower: 10)
            };

        var strategy = new DefaultDischargeStrategy();
        await strategy.ExecuteAsync(40, batteries.Select(b => b.Object).ToList(), CancellationToken.None);

        // Verify busy battery was skipped
        batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
        // Verify non-busy battery was used
        batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SkipsFullyDischargedBatteries()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(0, 100, false, maxDischargePower: 10), // Fully discharged
                TestHelpers.CreateBatteryMock(50, 100, false, maxDischargePower: 10)
            };

        var strategy = new DefaultDischargeStrategy();
        await strategy.ExecuteAsync(20, batteries.Select(b => b.Object).ToList(), CancellationToken.None);

        // Verify fully discharged battery was skipped and power was extracted from the other battery
        batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
        batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesExactDischargeMatch()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(50, 100, false, maxDischargePower: 10),
                TestHelpers.CreateBatteryMock(50, 100, false, maxDischargePower: 10)
            };

        var strategy = new DefaultDischargeStrategy();
        await strategy.ExecuteAsync(50, batteries.Select(b => b.Object).ToList(), CancellationToken.None);

        // Verify one battery was fully discharged
        batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
        batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesExcessDischargeRequest()
    {
        var batteries = new List<Mock<IBattery>>
            {
                TestHelpers.CreateBatteryMock(50, 100, false, maxDischargePower: 10),
                TestHelpers.CreateBatteryMock(30, 100, false, maxDischargePower: 10)
            };

        var strategy = new DefaultDischargeStrategy();
        await strategy.ExecuteAsync(100, batteries.Select(b => b.Object).ToList(), CancellationToken.None);

        foreach (var batteryMock in batteries)
        {
            batteryMock.Verify(b => b.SetNewPower(It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}

