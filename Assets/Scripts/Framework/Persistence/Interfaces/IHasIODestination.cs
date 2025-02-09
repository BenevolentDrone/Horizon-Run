namespace HereticalSolutions.Persistence
{
	public interface IHasIODestination
	{
		void EnsureIODestinationExists();

		bool IODestinationExists();

		void CreateIODestination();

		void EraseIODestination();
	}
}