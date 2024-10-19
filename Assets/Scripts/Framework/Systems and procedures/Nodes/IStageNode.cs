namespace HereticalSolutions.Systems
{
	public interface IStageNode<TProcedure>
		: IReadOnlyProcedureNode<TProcedure>
	{
		string Stage { get; }
	}
}