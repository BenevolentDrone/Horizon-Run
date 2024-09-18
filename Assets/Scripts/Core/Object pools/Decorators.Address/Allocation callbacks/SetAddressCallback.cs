using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    /// <summary>
    /// Represents a callback for setting the address of an allocated pool element.
    /// </summary>
    /// <typeparam name="T">The type of the pool element.</typeparam>
    public class SetAddressCallback<T>
        : IAllocationCallback<IPoolElementFacade<T>>
    {
        public string FullAddress { get; set; }

        public int[] AddressHashes { get; set; }

        private ILogger logger;
        
        public SetAddressCallback(
            string fullAddress = null,
            int[] addressHashes = null,
            ILogger logger = null)
        {
            FullAddress = fullAddress;
            
            AddressHashes = addressHashes;
            
            this.logger = logger;
        }

        public void OnAllocated(IPoolElementFacade<T> poolElementFacade)
        {
            if (FullAddress == null || AddressHashes == null)
                return;

            IPoolElementFacadeWithMetadata<T> facadeWithMetadata =
                poolElementFacade as IPoolElementFacadeWithMetadata<T>;

            if (facadeWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException<SetAddressCallback<T>>(
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
			
            var metadata = (AddressMetadata)
                facadeWithMetadata.Metadata.Get<IContainsAddress>();

            if (metadata == null)
            {
                throw new Exception(
                    logger.TryFormatException<SetAddressCallback<T>>(
                        "POOL ELEMENT FACADE HAS NO ADDRESS METADATA"));
            }
            
            metadata.FullAddress = FullAddress;
            
            metadata.AddressHashes = AddressHashes;
        }
    }
}