namespace HereticalSolutions.Pools
{
	public interface IAsyncPoolElementFacade<T>
	{
		T Value { get; set; }

		EPoolElementStatus Status { get; set; }

		IAsyncManagedPool<T> Pool { get; set; }

		void Push();
	}
}