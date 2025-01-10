namespace HereticalSolutions.Persistence
{
	public interface IStrategyWithIODestination
	{
		void EnsureIOTargetDestinationExists();

		bool IOTargetExists();

		void CreateIOTarget();

		void EraseIOTarget();
	}
}