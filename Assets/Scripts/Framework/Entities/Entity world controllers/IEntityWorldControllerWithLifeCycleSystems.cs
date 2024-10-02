namespace HereticalSolutions.Entities
{
	public interface IEntityWorldControllerWithLifeCycleSystems<TSystem, TWorld, TEntity>
		: IEntityWorldController<TWorld, TEntity>
	{
		TSystem EntityResolveSystems { get; set; }

		TSystem EntityInitializationSystems { get; set; }

		TSystem EntityDeinitializationSystems { get; set; }
	}
}