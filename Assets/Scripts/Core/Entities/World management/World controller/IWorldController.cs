namespace HereticalSolutions.Entities
{
    public interface IWorldController<TWorld, TSystem, TEntity>
    {
        TWorld World { get; }


        bool TrySpawnEntity(
            out TEntity entity);

        bool TrySpawnAndResolveEntity(
            object source,
            out TEntity entity);

        void DespawnEntity(
            TEntity entity);
    }
}