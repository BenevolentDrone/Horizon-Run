using System.Collections;

namespace HereticalSolutions.Pools
{
    public class PoolWithListCleanup<T>
        : ADecoratorPool<T>
    {
        public PoolWithListCleanup(
            IPool<T> innerPool)
            : base(innerPool)
        {
        }
        
        protected override void OnAfterPop(
            T instance,
            IPoolPopArgument[] args)
        {
            var instanceAsList = instance as IList;
            
            instanceAsList?.Clear();
        }
        
        protected override void OnBeforePush(
            T instance)
        {
            var instanceAsList = instance as IList;
            
            instanceAsList?.Clear();
        }
    }
}