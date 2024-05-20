namespace HereticalSolutions.Entities
{
	public interface IContainsEntityWorlds<TWorld, TWorldController>
	{
		IReadOnlyEntityWorldsRepository<TWorld, TWorldController> EntityWorldsRepository { get; }
	}
}