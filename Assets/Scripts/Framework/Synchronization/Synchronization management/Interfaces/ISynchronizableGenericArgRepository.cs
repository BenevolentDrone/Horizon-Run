namespace HereticalSolutions.Synchronization
{
	public interface ISynchronizableGenericArgRepository<TDelta>
		: IReadOnlySynchronizableGenericArgRepository<TDelta>
	{
		void AddSynchronizable(ISynchronizableGenericArg<TDelta> synchronizable);

		void RemoveSynchronizable(string id);

		void RemoveSynchronizable(ISynchronizableGenericArg<TDelta> synchronizable);

		void RemoveAllSynchronizables();
	}
}