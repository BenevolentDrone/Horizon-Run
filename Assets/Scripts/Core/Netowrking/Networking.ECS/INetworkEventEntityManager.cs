namespace HereticalSolutions.Networking.ECS
{
    public interface INetworkEventEntityManager<TEntity>
    {
        void ReplicateEventEntity(TEntity entity);
    }
}