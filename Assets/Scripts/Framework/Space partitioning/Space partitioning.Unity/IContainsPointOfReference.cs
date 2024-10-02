namespace HereticalSolutions.SpacePartitioning
{
	public interface IContainsPointOfReference<TValue>
	{
		TValue PointOfReference { get; set; }
	}
}