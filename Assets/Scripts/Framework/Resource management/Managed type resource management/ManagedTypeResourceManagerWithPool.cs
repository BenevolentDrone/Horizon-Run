using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;
using HereticalSolutions.Pools;

namespace HereticalSolutions.ResourceManagement
{
	public class ManagedTypeResourceManagerWithPool<TResource, THandle>
		: IManagedTypeResourceManager<TResource, THandle>
	{
		private readonly IRepository<THandle, TResource> resourceRepository;
		
		private readonly Queue<THandle> freeHandles;

		private readonly Func<THandle, THandle> newHandleAllocationDelegate;
		
		private readonly IPool<TResource> resourcePool;
		
		private readonly EqualityComparer<THandle> handleEqualityComparer;

		private readonly THandle uninitializedHandle;

		private readonly ILogger logger;
		
		private THandle lastAllocatedHandle;

		public ManagedTypeResourceManagerWithPool(
			IRepository<THandle, TResource> resourceRepository,
			Queue<THandle> freeHandles,
			Func<THandle, THandle> newHandleAllocationDelegate,
			IPool<TResource> resourcePool,
			THandle uninitializedHandle = default(THandle),
			ILogger logger)
		{
			this.resourceRepository = resourceRepository;
			
			this.freeHandles = freeHandles;

			this.newHandleAllocationDelegate = newHandleAllocationDelegate;
			
			this.resourcePool = resourcePool;
			
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

			resource = resourcePool.Pop();

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
			
			if (!resourceRepository.Has(handle))
			{
				return;
			}
			
			var resource = resourceRepository.Get(handle);
			
			resourcePool.Push(resource);

			resourceRepository.Remove(handle);
			
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

			if (!resourceRepository.Has(handle))
			{
				return false;
			}
			
			var resource = resourceRepository.Get(handle);
			
			resourcePool.Push(resource);
			
			resourceRepository.Remove(handle);

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