namespace HereticalSolutions.Lifetime
{
    public interface ILifetimeable
    {
        LifetimeController LifetimeController { get; }
        
        //Moved to Lifetime Controller instead
        /*
        void SetUp();
        
        void Initialize();
        
        void CleanUp();
        
        void TearDown();
        */
    }
}