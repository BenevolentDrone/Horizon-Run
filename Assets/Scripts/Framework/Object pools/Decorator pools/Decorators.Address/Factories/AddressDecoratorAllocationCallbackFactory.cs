using HereticalSolutions.Pools.AllocationCallbacks;

namespace HereticalSolutions.Pools.Factories
{
    public static class AddressDecoratorAllocationCallbackFactory
    {
        #region Allocation callbacks

        public static SetAddressCallback<T> BuildSetAddressCallback<T>(
            string fullAddress = null,
            int[] addressHashes = null)
        {
            return new SetAddressCallback<T>(fullAddress, addressHashes);
        }
        
        #endregion
    }
}