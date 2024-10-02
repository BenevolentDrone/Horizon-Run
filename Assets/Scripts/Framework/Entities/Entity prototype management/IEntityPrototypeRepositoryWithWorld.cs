namespace HereticalSolutions.Entities
{
	public interface IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity>
		: IEntityPrototypeRepository<TPrototypeID, TEntity>
	{
		TWorld PrototypeWorld { get; }
	}
}