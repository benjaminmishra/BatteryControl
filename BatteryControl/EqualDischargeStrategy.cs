namespace BatteryControl;

public class EqualDischargeStrategy : IDischargeStrategy
{
    public void Execute(int desiredOutputPower, IReadOnlyCollection<Battery> batteries)
    {
        throw new NotImplementedException();
    }
}