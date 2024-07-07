namespace HereticalSolutions.Networking.ECS
{
    public interface INetworkEntityManager<TEntityID, TEntity>
        : INetworkEntityRepository<TEntityID>,
          INetworkPrototypeEntityRepository,
          INetworkEventEntityManager<TEntity>
    {
        void StartInClientMode();
   
        void StartInServerMode();
        
        void Stop();
    }
}