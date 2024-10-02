namespace HereticalSolutions.Entities
{
    public interface IEntityWorldController<TWorld, TEntity>
    {
        TWorld World { get; }


        bool TrySpawnEntity(
            out TEntity entity);

        bool TrySpawnAndResolveEntity(
            object source,
            out TEntity entity);

        bool DespawnEntity(
            TEntity entity);
    }
}