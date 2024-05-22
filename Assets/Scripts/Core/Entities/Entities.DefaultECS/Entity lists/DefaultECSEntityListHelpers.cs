using System.Collections.Generic;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public static class DefaultECSEntityListHelpers
    {
        public static bool TryGetList(
            this DefaultECSEntityListManager entityListManager,
            ushort listHandle,
            out List<Entity> entityList)
        {
            entityList = null;
            
            if (listHandle == 0)
            {
                return false;
            }
            
            if (!entityListManager.HasList(listHandle))
            {
                return false;
            }
            
            entityList = entityListManager.GetList(listHandle);
            
            return true;
        }
    }
}