namespace HereticalSolutions.Persistence
{
	public interface IPopulateVisitorGeneric
	{
		bool Populate<TConcreteVisitable, TDTO>(
			TDTO DTO,
			ref TConcreteVisitable visitable);
	}
}