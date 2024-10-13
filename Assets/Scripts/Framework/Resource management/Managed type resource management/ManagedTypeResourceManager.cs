using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ManagedTypeResourceManager<TResource, THandle>
		: IManagedTypeResourceManager<TResource, THandle>
	{
		private readonly IRepository<THandle, TResource> resourceRepository;
		
		private readonly Queue<THandle> freeHandles;

		private readonly Func<THandle, THandle> newHandleAllocationDelegate;
		
		private readonly Func<TResource> newResourceAllocationDelegate;

		private readonly EqualityComparer<THandle> handleEqualityComparer;

		private readonly THandle uninitializedHandle;

		private readonly ILogger logger;
		
		private THandle lastAllocatedHandle;

		public ManagedTypeResourceManager(
			IRepository<THandle, TResource> resourceRepository,
			Queue<THandle> freeHandles,
			Func<THandle, THandle> newHandleAllocationDelegate,
			Func<TResource> newResourceAllocationDelegate,
			THandle uninitializedHandle = default(THandle),
			ILogger logger = null)
		{
			this.resourceRepository = resourceRepository;
			
			this.freeHandles = freeHandles;

			this.newHandleAllocationDelegate = newHandleAllocationDelegate;
			
			this.newResourceAllocationDelegate = newResourceAllocationDelegate;

			handleEqualityComparer = EqualityComparer<THandle>.Default;
			
			this.uninitializedHandle = uninitializedHandle;

			this.logger = logger;

			lastAllocatedHandle = uninitializedHandle;
		}

		#region IManagedTypeResourceManager

		public bool Has(
			THandle handle)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
				return false;
			
			return resourceRepository.Has(handle);
		}

		public TResource Get(
			THandle handle)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID HANDLE {handle}"));
			
			if (!resourceRepository.TryGet(
				handle,
				out var result))
			{
				return default(TResource);
			}

			return result;
		}

		public bool TryGet(
			THandle handle,
			out TResource resource)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
			{
				resource = default(TResource);

				return false;
			}

			return resourceRepository.TryGet(
				handle,
				out resource);
		}

		public bool TryAllocate(
			out THandle handle,
			out TResource resource)
		{
			if (freeHandles.Count > 0)
			{
				handle = freeHandles.Dequeue();
			}
			else
			{
				handle = newHandleAllocationDelegate(lastAllocatedHandle);
				
				lastAllocatedHandle = handle;
			}

			resource = newResourceAllocationDelegate.Invoke();

			resourceRepository.Add(
				handle,
				resource);
			
			logger?.Log(
				GetType(),
				$"ALLOCATED RESOURCE, HANDLE: {handle}");

			return true;
		}

		public void Remove(
			THandle handle)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
				return;
			
			if (!resourceRepository.TryRemove(handle))
			{
				return;
			}

			freeHandles.Enqueue(handle);
			
			logger?.Log(
				GetType(),
				$"REMOVED RESOURCE, HANDLE: {handle}");
		}

		public bool TryRemove(
			THandle handle)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
				return false;
			
			if (!resourceRepository.TryRemove(handle))
			{
				return false;
			}

			freeHandles.Enqueue(handle);
			
			logger?.Log(
				GetType(),
				$"REMOVED RESOURCE, HANDLE: {handle}");
			
            return true;
		}
		
		public IEnumerable<THandle> AllHandles
		{
			get => resourceRepository.Keys;
		}
        
		public IEnumerable<TResource> AllResources
		{
			get => resourceRepository.Values;
		}

		#endregion
	}
}