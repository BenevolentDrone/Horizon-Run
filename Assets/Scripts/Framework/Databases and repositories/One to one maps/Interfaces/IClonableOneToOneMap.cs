namespace HereticalSolutions.Repositories
{
	public interface IClonableOneToOneMap<TValue1, TValue2>
	{
		IOneToOneMap<TValue1, TValue2> Clone();
	}
}