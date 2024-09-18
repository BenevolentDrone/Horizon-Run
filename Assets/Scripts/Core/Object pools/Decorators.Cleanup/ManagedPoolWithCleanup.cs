using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Pools
{
    public class ManagedPoolWithCleanup<T>
        : ADecoratorManagedPool<T>
    {
        public ManagedPoolWithCleanup(
            IManagedPool<T> innerPool)
            : base(innerPool)
        {
        }
        
        protected override void OnAfterPop(
            IPoolElementFacade<T> instance,
            IPoolPopArgument[] args)
        {
            var valueAsCleanUppable = instance.Value as ICleanuppable;
            
            valueAsCleanUppable?.Cleanup();
        }
        
        protected override void OnBeforePush(
            IPoolElementFacade<T> instance)
        {
            var instanceAsCleanUppable = instance.Value as ICleanuppable;
            
            instanceAsCleanUppable?.Cleanup();
        }
    }
}