using System;

namespace HereticalSolutions.LifetimeManagement
{
    [Flags]
    public enum EInitializationFlags
    {
        NONE = 0,
        
        NO_ARGS_ALLOWED = 1 << 0,
        
        INITIALIZE_ON_PARENT_INITIALIZE = 1 << 1,
        INITIALIZE_WITH_PARENTS_ARGS = 1 << 2,
        
        INITIALIZE_CHILDREN_ON_INITIALIZE = 1 << 3,
    }
}