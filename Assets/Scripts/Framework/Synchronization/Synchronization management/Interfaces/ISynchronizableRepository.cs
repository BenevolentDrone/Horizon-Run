namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizableRepository
		: IReadOnlySynchronizableRepository
	{
		void AddSynchronizable(ISynchronizableNoArgs synchronizable);

		void RemoveSynchronizable(string id);

		void RemoveSynchronizable(ISynchronizableNoArgs synchronizable);

		void RemoveAllSynchronizables();
	}
}