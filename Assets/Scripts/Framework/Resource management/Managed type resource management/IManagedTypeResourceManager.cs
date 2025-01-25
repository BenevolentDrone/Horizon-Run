/*
using System.Collections.Generic;

namespace HereticalSolutions.ResourceManagement
{
    public interface IManagedTypeResourceManager<TResource, THandle>
    {
        bool Has(THandle handle);

        TResource Get(THandle handle);
        
        bool TryGet(
            THandle handle,
            out TResource resource);

        bool TryAllocate(
            out THandle handle,
            out TResource resource);

        void Remove(THandle handle);
        
        bool TryRemove(
            THandle handle);

        IEnumerable<THandle> AllHandles { get; }
        
        IEnumerable<TResource> AllResources { get; }
    }
}
*/