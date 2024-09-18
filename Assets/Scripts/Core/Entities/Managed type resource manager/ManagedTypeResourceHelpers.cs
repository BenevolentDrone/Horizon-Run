namespace HereticalSolutions.Entities
{
    public static class ManagedTypeResourceHelpers
    {
        public static void GetOrCreateResource<TEntityList, TListHandle>(
            this IManagedTypeResourceManager<TEntityList, TListHandle> entityListManager,
            ref TListHandle listHandle,
            out TEntityList entityList)
        {
            if (entityListManager.Has(listHandle))
            {
                entityList = entityListManager.Get(listHandle);
            }
            else
            {
                entityListManager.TryAllocate(
                    out listHandle,
                    out entityList);
            }
        }
    }
}