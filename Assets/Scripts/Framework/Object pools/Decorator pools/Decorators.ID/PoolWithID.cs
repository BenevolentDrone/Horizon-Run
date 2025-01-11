namespace HereticalSolutions.Pools.Decorators
{
    public class PoolWithID<T> : ADecoratorPool<T>
    {
        public string ID { get; private set; }

        public PoolWithID(
            IPool<T> innerPool,
            string id)
            : base(innerPool)
        {
            ID = id;
        }
    }
}