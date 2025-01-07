namespace HereticalSolutions.Pools
{
    public interface IPool<T>
    {
        T Pop();

        T Pop(
            IPoolPopArgument[] args);
        
        void Push(
            T instance);
    }
}