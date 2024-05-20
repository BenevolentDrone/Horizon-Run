using System.Collections.Generic;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public static class DefaultECSEntityListHelpers
    {
        public static bool TryGetList(
            this DefaultECSEntityListManager entityListManager,
            int listID,
            out List<Entity> entityList)
        {
            entityList = null;
            
            if (listID == 0)
            {
                return false;
            }
            
            if (!entityListManager.HasList(listID))
            {
                return false;
            }
            
            entityList = entityListManager.GetList(listID);
            
            return true;
        }
    }
}