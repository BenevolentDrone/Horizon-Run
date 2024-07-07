using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
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
		private readonly INonAllocDecoratedPool<GameObject> physicsBodiesPool;

		private readonly Queue<ushort> freeHandles;

		private readonly IRepository<ushort, UnityPhysicsBodyDescriptor> physicsBodiesRepository;

		private readonly AddressArgument addressArgument;

		private readonly IPoolDecoratorArgument[] arguments;

		private readonly ILogger logger;

		private ushort nextHandleToAllocate;

		public UnityPhysicsManager(
			INonAllocDecoratedPool<GameObject> physicsBodiesPool,
			Queue<ushort> freeHandles,
			IRepository<ushort, UnityPhysicsBodyDescriptor> physicsBodiesRepository,
			ILogger logger = null)
		{
			this.physicsBodiesPool = physicsBodiesPool;

			this.freeHandles = freeHandles;

			this.physicsBodiesRepository = physicsBodiesRepository;

			this.logger = logger;

			nextHandleToAllocate = 1;

			addressArgument = new AddressArgument();

			arguments = new IPoolDecoratorArgument[]
			{
				addressArgument
			};
		}

		#region IPhysicsManager

		public bool HasPhysicsBody(ushort rigidBodyHandle)
		{
			if (rigidBodyHandle == 0)
				return false;

			return physicsBodiesRepository.Has(rigidBodyHandle);
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
					logger.TryFormat<UnityPhysicsManager>(
						$"POOLED ELEMENT'S VALUE IS NULL"));
			}

			if (pooledPhysicsBodyElement.Status != EPoolElementStatus.POPPED)
			{
				throw new Exception(
					logger.TryFormat<UnityPhysicsManager>(
						$"POOLED ELEMENT'S STATUS IS INVALID ({pooledPhysicsBodyElement.Value.name})"));
			}

			if (!pooledPhysicsBodyElement.Value.activeInHierarchy)
			{
				throw new Exception(
					logger.TryFormat<UnityPhysicsManager>(
						$"POOLED GAME OBJECT IS SPAWNED DISABLED ({pooledPhysicsBodyElement.Value.name})"));
			}

			var gameObject = pooledPhysicsBodyElement.Value;

			physicsBody = gameObject.GetComponent<UnityPhysicsBodyDescriptor>();

			if (physicsBody == null)
			{
				throw new Exception(
					logger.TryFormat<UnityPhysicsManager>(
						$"PHYSICS BODY DESCRIPTOR IS NULL"));
			}

			physicsBody.PoolElement = pooledPhysicsBodyElement;

			physicsBodiesRepository.Add(
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
					logger.TryFormat<UnityPhysicsManager>(
						$"INVALID PHYSICS BODY HANDLE {physicsBodyHandle}"));

			var result = physicsBodiesRepository.TryGet(
				physicsBodyHandle,
				out physicsBody);

			return result;
		}

		public bool TryDestroyPhysicsBody(
			ushort physicsBodyHandle)
		{
			if (physicsBodyHandle == 0)
				return false;

			if (!physicsBodiesRepository.TryGet(
				physicsBodyHandle,
				out var physicsBody))
			{
				return false;
			}

			var poolElement = physicsBody.PoolElement;

			if (poolElement == null)
			{
				throw new Exception(
					logger.TryFormat<UnityPhysicsManager>(
						$"POOL ELEMENT IS NULL"));
			}

			physicsBody.PoolElement = null;

			poolElement.Push();

			physicsBodiesRepository.TryRemove(physicsBodyHandle);

			freeHandles.Enqueue(physicsBodyHandle);

			logger?.Log<UnityPhysicsManager>(
				$"REMOVED LIST {physicsBodyHandle}");

			return true;
		}

		#endregion
	}
}