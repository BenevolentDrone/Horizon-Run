namespace HereticalSolutions.Entities
{
	public interface IPrototypeCompliantWorldController<TWorld, TEntity>
	{
		IPrototypesRepository<TWorld, TEntity> PrototypeRepository { get; }


		bool TrySpawnEntityFromPrototype(
			string prototypeID,
			out TEntity entity);
		
		bool TrySpawnEntityFromPrototype(
			string prototypeID,
			TEntity @override,
			out TEntity entity);

		bool TrySpawnAndResolveEntityFromPrototype(
			string prototypeID,
			object source,
			out TEntity entity);
		
		bool TrySpawnAndResolveEntityFromPrototype(
			string prototypeID,
			TEntity @override,
			object source,
			out TEntity entity);
	}
}