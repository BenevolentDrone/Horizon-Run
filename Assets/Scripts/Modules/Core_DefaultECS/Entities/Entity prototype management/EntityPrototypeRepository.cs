using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Entities;


using TWorld = DefaultEcs.World;

using TPrototypeID = System.String;

using TEntity = DefaultEcs.Entity;


namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class EntityPrototypeRepository
		: IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity>
	{
		private readonly IRepository<TPrototypeID, TEntity> prototypeRepository;

		public EntityPrototypeRepository(
			TWorld prototypeWorld,
			IRepository<TPrototypeID, TEntity> prototypeRepository)
		{
			PrototypeWorld = prototypeWorld;

			this.prototypeRepository = prototypeRepository;
		}

		#region IEntityPrototypeRepository

		public bool HasPrototype(TPrototypeID prototypeID)
		{
			return prototypeRepository.Has(prototypeID);
		}

		public bool TryGetPrototype(
			TPrototypeID prototypeID,
			out TEntity prototypeEntity)
		{
			return prototypeRepository.TryGet(
				prototypeID,
				out prototypeEntity);
		}

		public bool TryAllocatePrototype(
			TPrototypeID prototypeID,
			out TEntity prototypeEntity)
		{
			prototypeEntity = PrototypeWorld.CreateEntity();

			if (!prototypeRepository.TryAdd(
				prototypeID,
				prototypeEntity))
			{
				prototypeEntity.Dispose();

				return false;
			}

			return true;
		}

		public bool RemovePrototype(
			TPrototypeID prototypeID)
		{
			return prototypeRepository.TryRemove(
				prototypeID);
		}

		public IEnumerable<TPrototypeID> AllPrototypeIDs { get => prototypeRepository.Keys; }

		#endregion

		#region IEntityPrototypeRepositoryWithWorld

		public TWorld PrototypeWorld { get; private set; }

		#endregion
	}
}