namespace HereticalSolutions.Persistence
{
	public interface IStrategyWithState
	{
		void InitializeRead();

		void FinalizeRead();


		void InitializeWrite();

		void FinalizeWrite();
		

		void InitializeAppend();

		void FinalizeAppend();
	}
}