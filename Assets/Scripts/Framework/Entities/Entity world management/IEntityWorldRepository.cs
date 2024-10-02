namespace HereticalSolutions.Entities
{
    public interface IEntityWorldRepository<TWorldID, TWorld, TEntity>
        : IReadOnlyEntityWorldRepository<TWorldID, TWorld, TEntity>
    {
        bool AddWorld(
            TWorldID worldID,
            IEntityWorldController<TWorld, TEntity> worldController);

        
        bool RemoveWorld(TWorldID worldID);

        bool RemoveWorld(TWorld world);
    }
}