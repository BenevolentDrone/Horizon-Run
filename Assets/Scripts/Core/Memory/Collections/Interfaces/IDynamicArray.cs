namespace HereticalSolutions.Collections
{
    public interface IDynamicArray<T>
    {
        int Count { get; }

        int Capacity { get; }

        T ElementAt(int index);
        
        T this[int index] { get; }

        T Get(int index);
    }
}