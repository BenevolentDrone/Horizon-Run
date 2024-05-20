using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class UnityDecoratorsPoolsFactory
    {
        #region Allocation callbacks

        public static RenameByStringAndIndexCallback BuildRenameByStringAndIndexCallback(string name)
        {
            return new RenameByStringAndIndexCallback(name);
        }

        #endregion
    }
}