namespace HereticalSolutions.StanleyScript
{
	public interface IStackMachine
		: IContainsScopes,
		  IContainsInterpreter,
		  IContainsInstructionHandlers,
		  IContainsTypecasters,
		  IContainsProgramControls
	{
		int StackSize { get; }

		int ProgramCounter { get; }

		#region Stack operations

		void Push(
			IStanleyVariable variable);

		bool Pop(
			out IStanleyVariable variable);

		bool Peek(
			out IStanleyVariable variable);


		bool PushToTop(
			int offset,
			IStanleyVariable variable);

		bool PopFromTop(
			int offset,
			out IStanleyVariable variable);

		bool PeekFromTop(
			int offset,
			out IStanleyVariable variable);


		bool PushToBottom(
			int offset,
			IStanleyVariable variable);

		bool PopFromBottom(
			int offset,
			out IStanleyVariable variable);

		bool PeekFromBottom(
			int offset,
			out IStanleyVariable variable);

		#endregion

		void Clear();
	}
}