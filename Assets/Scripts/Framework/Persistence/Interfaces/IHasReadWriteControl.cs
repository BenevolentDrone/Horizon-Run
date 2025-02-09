namespace HereticalSolutions.Persistence
{
	public interface IHasReadWriteControl
	{
		bool SupportsSimultaneousReadAndWrite { get; }
		

		void InitializeRead();

		void FinalizeRead();


		void InitializeWrite();

		void FinalizeWrite();
		

		void InitializeAppend();

		void FinalizeAppend();


		void InitializeReadAndWrite();

		void FinalizeReadAndWrite();
	}
}