namespace HereticalSolutions.Synchronization
{
	public interface IReadOnlySynchronizableRepository
	{
		bool TryGetSynchronizable(
			string id,
			out ISynchronizableNoArgs synchronizable);
	}
}