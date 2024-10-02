namespace HereticalSolutions.Entities
{
	public interface IEntityWorldControllerWithPrototypes<TWorld, TPrototypeID, TEntity>
		: IEntityWorldController<TWorld, TEntity>
	{
		IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity> PrototypeRepository { get; }


		bool TrySpawnEntityFromPrototype(
			TPrototypeID prototypeID,
			out TEntity entity);
		
		bool TrySpawnEntityFromPrototype(
			TPrototypeID prototypeID,
			TEntity @override,
			out TEntity entity);

		bool TrySpawnAndResolveEntityFromPrototype(
			TPrototypeID prototypeID,
			object source,
			out TEntity entity);
		
		bool TrySpawnAndResolveEntityFromPrototype(
			TPrototypeID prototypeID,
			TEntity @override,
			object source,
			out TEntity entity);
	}
}