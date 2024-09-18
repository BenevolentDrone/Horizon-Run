namespace HereticalSolutions.LifetimeManagement
{
    public interface IContainsLifetime
    {
        ILifetimeable Lifetime { get; }
    }
}