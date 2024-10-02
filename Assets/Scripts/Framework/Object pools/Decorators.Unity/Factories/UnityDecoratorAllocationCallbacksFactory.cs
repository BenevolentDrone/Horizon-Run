using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    public static class UnityDecoratorAllocationCallbacksFactory
    {
        public static RenameByStringAndIndexCallback BuildRenameByStringAndIndexCallback(string name)
        {
            return new RenameByStringAndIndexCallback(name);
        }
    }
}