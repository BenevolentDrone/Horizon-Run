namespace HereticalSolutions.Pools
{
	public interface IAsyncManagedPool<T>
		: IAsyncPool<IPoolElementFacade<T>>
	{
	}
}