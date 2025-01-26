using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class AddressDecoratorAllocationCallbackFactory
    {
        #region Allocation callbacks

        public static SetAddressCallback<T> BuildSetAddressCallback<T>(
            ILoggerResolver loggerResolver,
            string fullAddress = null,
            int[] addressHashes = null)
        {
            return new SetAddressCallback<T>(
                loggerResolver?.GetLogger<SetAddressCallback<T>>(),
                fullAddress,
                addressHashes);
        }
        
        #endregion
    }
}