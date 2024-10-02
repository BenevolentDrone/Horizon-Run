using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Pools
{
    public class PoolWithCleanup<T>
        : ADecoratorPool<T>
    {
        public PoolWithCleanup(
            IPool<T> innerPool)
            : base(innerPool)
        {
        }
        
        protected override void OnAfterPop(
        	T instance,
        	IPoolPopArgument[] args)
        {
            var instanceAsCleanUppable = instance as ICleanuppable;
            
            instanceAsCleanUppable?.Cleanup();
        }
        
        protected override void OnBeforePush(
            T instance)
        {
            var instanceAsCleanUppable = instance as ICleanuppable;
            
            instanceAsCleanUppable?.Cleanup();
        }
    }
}