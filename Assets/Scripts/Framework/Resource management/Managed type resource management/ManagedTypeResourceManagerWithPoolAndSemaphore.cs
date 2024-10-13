using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ManagedTypeResourceManagerWithPoolAndSemaphore<TResource, THandle>
		: IManagedTypeResourceManager<TResource, THandle>
	{
		private readonly IRepository<THandle, TResource> resourceRepository;
		
		private readonly Queue<THandle> freeHandles;

		private readonly Func<THandle, THandle> newHandleAllocationDelegate;
		
		private readonly IPool<TResource> resourcePool;
		
		private readonly EqualityComparer<THandle> handleEqualityComparer;

		private readonly THandle uninitializedHandle;

		private readonly SemaphoreSlim semaphore;
        
		private readonly ILogger logger;
		
		private THandle lastAllocatedHandle;

		public ManagedTypeResourceManagerWithPoolAndSemaphore(
			IRepository<THandle, TResource> resourceRepository,
			Queue<THandle> freeHandles,
			Func<THandle, THandle> newHandleAllocationDelegate,
			IPool<TResource> resourcePool,
			SemaphoreSlim semaphore,
			THandle uninitializedHandle = default(THandle),
			ILogger logger = null)
		{
			this.resourceRepository = resourceRepository;
			
			this.freeHandles = freeHandles;

			this.newHandleAllocationDelegate = newHandleAllocationDelegate;
			
			this.resourcePool = resourcePool;
			
			handleEqualityComparer = EqualityComparer<THandle>.Default;
			
			this.semaphore = semaphore;
            
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
			
			bool result = false;
			
			semaphore.Wait();
			
			result = resourceRepository.Has(handle);
			
			semaphore.Release();

			return result;
		}

		public TResource Get(
			THandle handle)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID HANDLE {handle}"));

			TResource result = default(TResource);
			
			semaphore.Wait();
			
			if (!resourceRepository.TryGet(
				handle,
				out result))
			{
				result = default(TResource);
			}

			semaphore.Release();

			return result;
		}

		public bool TryGet(
			THandle handle,
			out TResource resource)
		{
			resource = default(TResource);
			
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
			{
				return false;
			}
			
			bool result = false;
			
			semaphore.Wait();
			
			result = resourceRepository.TryGet(
				handle,
				out resource);

			semaphore.Release();

			return result;
		}

		public bool TryAllocate(
			out THandle handle,
			out TResource resource)
		{
			semaphore.Wait();
            
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

			semaphore.Release();
			
			return true;
		}

		public void Remove(
			THandle handle)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
				return;
			
			semaphore.Wait();
			
			if (resourceRepository.Has(handle))
			{
				var resource = resourceRepository.Get(handle);

				resourcePool.Push(resource);

				resourceRepository.Remove(handle);

				freeHandles.Enqueue(handle);

				logger?.Log(
					GetType(),
					$"REMOVED RESOURCE, HANDLE: {handle}");
			}

			semaphore.Release();
		}

		public bool TryRemove(
			THandle handle)
		{
			if (handleEqualityComparer.Equals(handle, uninitializedHandle))
				return false;

			semaphore.Wait();

			if (resourceRepository.Has(handle))
			{

				var resource = resourceRepository.Get(handle);

				resourcePool.Push(resource);

				resourceRepository.Remove(handle);

				freeHandles.Enqueue(handle);

				logger?.Log(
					GetType(),
					$"REMOVED RESOURCE, HANDLE: {handle}");
			}

			semaphore.Release();
			
            return true;
		}
		
		public IEnumerable<THandle> AllHandles
		{
			get
			{
				IEnumerable<THandle> result = null;

				semaphore.Wait();
				
				result = resourceRepository.Keys.ToArray();

				semaphore.Release();

				return result;
			}
		}
        
		public IEnumerable<TResource> AllResources
		{
			get
			{
				IEnumerable<TResource> result = null;

				semaphore.Wait();
				
				result = resourceRepository.Values.ToArray();

				semaphore.Release();

				return result;
			}
		}

		#endregion
	}
}