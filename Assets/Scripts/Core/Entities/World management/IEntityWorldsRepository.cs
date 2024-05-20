namespace HereticalSolutions.Entities
{
    public interface IEntityWorldsRepository<TWorld, TWorldController>
        : IReadOnlyEntityWorldsRepository<TWorld, TWorldController>
    {
        void AddWorld(
            string worldID,
            TWorldController worldController);

        
        void RemoveWorld(string worldID);

        void RemoveWorld(TWorld world);
    }
}