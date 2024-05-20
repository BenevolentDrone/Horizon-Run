namespace HereticalSolutions.Entities
{
	public interface IRegistryCompliantWorldController<TEntity>
	{
		bool TryGetEntityFromRegistry(
			TEntity registryEntity,
			out TEntity localEntity);

		bool TrySpawnEntityFromRegistry(
			TEntity registryEntity,
			out TEntity localEntity);
		
		bool TrySpawnEntityFromRegistry(
			TEntity registryEntity,
			TEntity overrideEntity,
			out TEntity localEntity);

		bool TrySpawnAndResolveEntityFromRegistry(
			TEntity registryEntity,
			object source,
			out TEntity localEntity);
		
		bool TrySpawnAndResolveEntityFromRegistry(
			TEntity registryEntity,
			TEntity overrideEntity,
			object source,
			out TEntity localEntity);

		bool TrySpawnEntityFromPrototypeAndLinkToRegistry(
			TEntity registryEntity,
			string prototypeID,
			out TEntity localEntity);

		bool TrySpawnAndResolveEntityFromPrototypeAndLinkToRegistry(
			TEntity registryEntity,
			string prototypeID,
			object source,
			out TEntity localEntity);

		void DespawnEntityAndUnlinkFromRegistry(
			TEntity registryEntity);

		bool TryReplaceEntityFromPrototypeAndUpdateRegistry(
			TEntity registryEntity,
			string prototypeID,
			out TEntity localEntity);

		bool TryReplaceAndResolveEntityFromPrototypeAndUpdateRegistry(
			TEntity registryEntity,
			string prototypeID,
			object source,
			out TEntity localEntity);
	}
}