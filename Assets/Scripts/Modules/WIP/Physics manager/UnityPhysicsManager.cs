using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class UnityPhysicsManager
		: IPhysicsManager<ushort, UnityPhysicsBodyDescriptor>
	{
		//LET THE PHYSICS BODIES HIT THE FLOOR
		//LET THE PHYSICS BODIES HIT THE FLOOR
		//LET THE PHYSICS BODIES HIT THE FLOOR
		//LET THE PHYSICS BODIES HIT THE
		//...
		//...
		//FLOOOOOOOOOOOOOR!
		private readonly IManagedPool<GameObject> physicsBodiesPool;

		private readonly Queue<ushort> freeHandles;

		private readonly IRepository<ushort, UnityPhysicsBodyDescriptor> physicsBodyRepository;

		private readonly AddressArgument addressArgument;

		private readonly IPoolPopArgument[] arguments;

		private readonly ILogger logger;

		private ushort nextHandleToAllocate;

		public UnityPhysicsManager(
			IManagedPool<GameObject> physicsBodiesPool,
			Queue<ushort> freeHandles,
			IRepository<ushort, UnityPhysicsBodyDescriptor> physicsBodyRepository,
			ILogger logger)
		{
			this.physicsBodiesPool = physicsBodiesPool;

			this.freeHandles = freeHandles;

			this.physicsBodyRepository = physicsBodyRepository;

			this.logger = logger;

			nextHandleToAllocate = 1;

			addressArgument = new AddressArgument();

			arguments = new IPoolPopArgument[]
			{
				addressArgument
			};
		}

		#region IPhysicsManager

		public bool HasPhysicsBody(ushort rigidBodyHandle)
		{
			if (rigidBodyHandle == 0)
				return false;

			return physicsBodyRepository.Has(rigidBodyHandle);
		}

		public bool SpawnPhysicsBody(
			string prototypeID,
			out ushort physicsBodyHandle,
			out UnityPhysicsBodyDescriptor physicsBody)
		{
			if (freeHandles.Count > 0)
			{
				physicsBodyHandle = freeHandles.Dequeue();
			}
			else
			{
				physicsBodyHandle = nextHandleToAllocate++;
			}

			addressArgument.FullAddress = prototypeID;
			addressArgument.AddressHashes = prototypeID.AddressToHashes();

			var pooledPhysicsBodyElement = physicsBodiesPool
				.Pop(arguments);

			if (pooledPhysicsBodyElement.Value == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOLED ELEMENT'S VALUE IS NULL"));
			}

			if (pooledPhysicsBodyElement.Status != EPoolElementStatus.POPPED)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOLED ELEMENT'S STATUS IS INVALID ({pooledPhysicsBodyElement.Value.name})"));
			}

			if (!pooledPhysicsBodyElement.Value.activeInHierarchy)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOLED GAME OBJECT IS SPAWNED DISABLED ({pooledPhysicsBodyElement.Value.name})"));
			}

			var gameObject = pooledPhysicsBodyElement.Value;

			physicsBody = gameObject.GetComponent<UnityPhysicsBodyDescriptor>();

			if (physicsBody == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PHYSICS BODY DESCRIPTOR IS NULL"));
			}

			physicsBody.PoolElement = pooledPhysicsBodyElement;

			physicsBodyRepository.Add(
				physicsBodyHandle,
				physicsBody);

			logger?.Log<UnityPhysicsManager>(
				$"SPAWNED PHYSICS BODY: {physicsBodyHandle}, PROTOTYPE ID: {prototypeID}");

			return true;
		}

		public bool TryGetPhysicsBody(
			ushort physicsBodyHandle,
			out UnityPhysicsBodyDescriptor physicsBody)
		{
			if (physicsBodyHandle == 0)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID PHYSICS BODY HANDLE {physicsBodyHandle}"));

			var result = physicsBodyRepository.TryGet(
				physicsBodyHandle,
				out physicsBody);

			return result;
		}

		public bool TryDestroyPhysicsBody(
			ushort physicsBodyHandle)
		{
			if (physicsBodyHandle == 0)
				return false;

			if (!physicsBodyRepository.TryGet(
				physicsBodyHandle,
				out var physicsBody))
			{
				return false;
			}

			var poolElement = physicsBody.PoolElement;

			if (poolElement == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"POOL ELEMENT IS NULL"));
			}

			physicsBody.PoolElement = null;

			poolElement.Push();

			physicsBodyRepository.TryRemove(physicsBodyHandle);

			freeHandles.Enqueue(physicsBodyHandle);

			logger?.Log<UnityPhysicsManager>(
				$"REMOVED LIST {physicsBodyHandle}");

			return true;
		}

		#endregion
	}
}