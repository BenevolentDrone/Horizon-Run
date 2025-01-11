namespace HereticalSolutions.Allocations
{
    public interface IAllocationCallback<T>
    {
        void OnAllocated(
            T instance);
    }
}