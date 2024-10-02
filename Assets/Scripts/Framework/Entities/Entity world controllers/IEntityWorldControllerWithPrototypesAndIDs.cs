namespace HereticalSolutions.Entities
{
	public interface IEntityWorldControllerWithPrototypesAndIDs<TWorld, TPrototypeID, TEntityID, TEntity>
		: IEntityWorldControllerWithPrototypes<TWorld, TPrototypeID, TEntity>
	{
		bool TrySpawnEntityWithIDFromPrototype(
			TPrototypeID prototypeID,
			TEntityID entityID,
			out TEntity entity);
		
		bool TrySpawnEntityWithIDFromPrototype(
			TPrototypeID prototypeID,
			TEntityID entityID,
			TEntity @override,
			out TEntity entity);

		bool TrySpawnAndResolveEntityWithIDFromPrototype(
			TPrototypeID prototypeID,
			TEntityID entityID,
			object source,
			out TEntity entity);
		
		bool TrySpawnAndResolveEntityWithIDFromPrototype(
			TPrototypeID prototypeID,
			TEntityID entityID,
			TEntity @override,
			object source,
			out TEntity entity);
	}
}