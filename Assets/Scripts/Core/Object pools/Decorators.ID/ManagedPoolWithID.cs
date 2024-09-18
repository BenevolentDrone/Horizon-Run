using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
    public class ManagedPoolWithID<T> : ADecoratorManagedPool<T>
    {
        public string ID { get; private set; }

        public ManagedPoolWithID(
            IManagedPool<T> innerPool,
            string id,
            ILogger logger = null)
            : base(
                innerPool,
                logger)
        {
            ID = id;
        }
    }
}