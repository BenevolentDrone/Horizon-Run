using System.Collections.Generic;

namespace HereticalSolutions.Entities
{
	public interface IReadOnlyMultiWorldEntityManager<TWorldID, TEntityID, TEntity>
		: IReadOnlyEntityManager<TEntityID, TEntity>
	{
		bool HasEntity(
			TEntityID entityID,
			TWorldID worldID);

		bool TryGetRegistryEntity(
			TEntityID entityID,
			out TEntity registryEntity);

		bool TryGetEntity(
			TEntityID entityID,
			TWorldID worldID,
			out TEntity entity);

		IEnumerable<TEntity> AllRegistryEntities { get; }
	}
}