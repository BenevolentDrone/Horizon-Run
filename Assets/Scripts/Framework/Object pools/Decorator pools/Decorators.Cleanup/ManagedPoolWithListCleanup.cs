using System.Collections;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
    public class ManagedPoolWithListCleanup<T>
        : ADecoratorManagedPool<T>
    {
        public ManagedPoolWithListCleanup(
            IManagedPool<T> innerPool,
            ILogger logger)
            : base(
                innerPool,
                logger)
        {
        }
        
        protected override void OnAfterPop(
            IPoolElementFacade<T> instance,
            IPoolPopArgument[] args)
        {
            var instanceAsList = instance.Value as IList;
            
            instanceAsList?.Clear();
        }
        
        protected override void OnBeforePush(
            IPoolElementFacade<T> instance)
        {
            var instanceAsList = instance.Value as IList;
            
            instanceAsList?.Clear();
        }
    }
}