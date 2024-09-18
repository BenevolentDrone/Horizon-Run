namespace HereticalSolutions.LifetimeManagement
{
    public interface ILifetimeProvideable
    {
        ILifetimeable Lifetime { set; }
    }
}