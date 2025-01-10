using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    public static class UnityDecoratorAllocationCallbackFactory
    {
        public static RenameByStringAndIndexCallback BuildRenameByStringAndIndexCallback(string name)
        {
            return new RenameByStringAndIndexCallback(name);
        }
    }
}