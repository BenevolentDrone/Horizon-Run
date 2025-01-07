namespace HereticalSolutions.Pools
{
    public interface IPoolElementFacade<T>
    {
        T Value { get; set; }

        EPoolElementStatus Status { get; set; }

        IManagedPool<T> Pool { get; set; }

        void Push();
    }
}