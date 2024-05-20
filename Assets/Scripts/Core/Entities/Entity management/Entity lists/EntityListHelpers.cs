namespace HereticalSolutions.Entities
{
    public static class EntityListHelpers
    {
        public static void GetOrCreateList<TListID, TEntityList>(
            this IEntityListManager<TListID, TEntityList> entityListManager,
            ref TListID listID,
            out TEntityList entityList)
        {
            if (entityListManager.HasList(listID))
            {
                entityList = entityListManager.GetList(listID);
            }
            else
            {
                entityListManager.CreateList(
                    out listID,
                    out entityList);
            }
        }
    }
}