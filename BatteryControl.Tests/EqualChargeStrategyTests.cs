using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace BatteryControl.Tests
{
    [Trait("Category", "Unit")]
    [Trait("Type", "EqualChargeStrategy")]
    public class EqualChargeStrategyTests
    {
        [Fact]
        public async Task ExecuteAsync_AllocatesPowerToBatteriesInOrderOfCharge()
        {
            var batteries = new List<Mock<Battery>>
            {
                CreateBatteryMock(50, 100, false),
                CreateBatteryMock(20, 100, false),
                CreateBatteryMock(80, 100, false)
            };

            var strategy = new EqualChargeStrategy();
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
            var batteries = new List<Mock<Battery>>
            {
                CreateBatteryMock(100, 100, false),
                CreateBatteryMock(90, 100, false)
            };

            var strategy = new EqualChargeStrategy();
            var inputPower = 50;
            var cancellationToken = CancellationToken.None;

            await strategy.ExecuteAsync(inputPower, batteries.Select(b => b.Object).ToList(), cancellationToken);

            batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
            batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_HandlesBusyBatteries()
        {
            var batteries = new List<Mock<Battery>>
            {
                CreateBatteryMock(50, 100, true),
                CreateBatteryMock(20, 100, false)
            };

            var strategy = new EqualChargeStrategy();
            var inputPower = 50;
            var cancellationToken = CancellationToken.None;

            await strategy.ExecuteAsync(inputPower, batteries.Select(b => b.Object).ToList(), cancellationToken);

            batteries[0].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Never);
            batteries[1].Verify(b => b.SetNewPower(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ThrowsIfCancelled()
        {
            var batteries = new List<Mock<Battery>>
            {
                CreateBatteryMock(50, 100, false)
            };

            var strategy = new EqualChargeStrategy();
            var inputPower = 50;
            using var cts = new CancellationTokenSource();

            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(
                () => strategy.ExecuteAsync(inputPower, batteries.Select(b => b.Object).ToList(), cts.Token));
        }

        private static Mock<Battery> CreateBatteryMock(int batteryPercent, int maxChargePower, bool isBusy)
        {
            var mock = new Mock<Battery>();

            mock.Setup(b => b.GetBatteryPercent()).Returns(batteryPercent);
            mock.SetupGet(b => b.MaxChargePower).Returns(maxChargePower);
            mock.Setup(b => b.IsBusy()).Returns(isBusy);
            mock.Setup(b => b.SetNewPower(It.IsAny<int>())).Returns(Task.CompletedTask);

            return mock;
        }
    }
}
