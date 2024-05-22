namespace HereticalSolutions.Entities
{
    public static class EntityListHelpers
    {
        public static void GetOrCreateList<TListHandle, TEntityList>(
            this IEntityListManager<TListHandle, TEntityList> entityListManager,
            ref TListHandle listHandle,
            out TEntityList entityList)
        {
            if (entityListManager.HasList(listHandle))
            {
                entityList = entityListManager.GetList(listHandle);
            }
            else
            {
                entityListManager.CreateList(
                    out listHandle,
                    out entityList);
            }
        }
    }
}