namespace HereticalSolutions.Synchronization
{
	public interface IReadOnlySynchronizableGenericArgRepository<TDelta>
	{
		bool TryGetSynchronizable(
			string id,
			out ISynchronizableGenericArg<TDelta> synchronizable);
	}
}