namespace HereticalSolutions.Entities
{
	public interface IContainsEntityInitializationSystems<TSystem>
	{
		TSystem EntityResolveSystems { get; }

		TSystem EntityInitializationSystems { get; }

		TSystem EntityDeinitializationSystems { get; }

		void Initialize(
			TSystem resolveSystems,
			TSystem initializationSystems,
			TSystem deinitializationSystems);
	}
}