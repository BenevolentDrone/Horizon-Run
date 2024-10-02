namespace HereticalSolutions.Entities
{
    public struct WorldOverrideDescriptor<TWorldID, TEntity>
    {
        public TWorldID WorldID;

        public TEntity OverrideEntity;
    }
}