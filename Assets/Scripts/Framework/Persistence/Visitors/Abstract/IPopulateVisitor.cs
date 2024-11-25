namespace HereticalSolutions.Persistence
{
	public interface IPopulateVisitor<TDTO>
		: IConcreteVisitor
	{
		bool Populate(
			TDTO DTO,
			ref IVisitable visitable);
	}
}