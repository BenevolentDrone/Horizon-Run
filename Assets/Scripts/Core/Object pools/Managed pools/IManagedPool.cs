namespace HereticalSolutions.Pools
{
	public interface IManagedPool<T>
		: IPool<IPoolElementFacade<T>>
	{
	}
}