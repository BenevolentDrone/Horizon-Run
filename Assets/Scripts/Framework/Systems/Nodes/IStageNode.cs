namespace HereticalSolutions.Systems
{
	public interface IStageNode<TSystem>
		: IReadOnlySystemNode<TSystem>
	{
		string Stage { get; }
	}
}