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
		private readonly IRepository<TPrototypeID, TEntity> prototypesRepository;

		public EntityPrototypeRepository(
			TWorld prototypeWorld,
			IRepository<TPrototypeID, TEntity> prototypesRepository)
		{
			PrototypeWorld = prototypeWorld;

			this.prototypesRepository = prototypesRepository;
		}

		#region IEntityPrototypeRepository

		public bool HasPrototype(TPrototypeID prototypeID)
		{
			return prototypesRepository.Has(prototypeID);
		}

		public bool TryGetPrototype(
			TPrototypeID prototypeID,
			out TEntity prototypeEntity)
		{
			return prototypesRepository.TryGet(
				prototypeID,
				out prototypeEntity);
		}

		public bool TryAllocatePrototype(
			TPrototypeID prototypeID,
			out TEntity prototypeEntity)
		{
			prototypeEntity = PrototypeWorld.CreateEntity();

			if (!prototypesRepository.TryAdd(
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
			return prototypesRepository.TryRemove(
				prototypeID);
		}

		public IEnumerable<TPrototypeID> AllPrototypeIDs { get => prototypesRepository.Keys; }

		#endregion

		#region IEntityPrototypeRepositoryWithWorld

		public TWorld PrototypeWorld { get; private set; }

		#endregion
	}
}