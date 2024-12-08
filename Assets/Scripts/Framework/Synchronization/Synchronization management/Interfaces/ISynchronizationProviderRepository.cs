namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizationProviderRepository
	{
		bool TryGetProvider(
			string id,
			out ISynchronizationProvider provider);
	}
}