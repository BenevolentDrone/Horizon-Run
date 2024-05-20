namespace HereticalSolutions.StanleyScript
{
	public interface IStackMachine
	{
		int StackSize { get; }

		void Push(
			IStanleyVariable variable);

		bool Pop(
			out IStanleyVariable variable);

		public bool Peek(
			out IStanleyVariable variable);

		public bool PeekFromTop(
			int relativeIndex,
			out IStanleyVariable variable);

		public bool PeekFromBottom(
			int relativeIndex,
			out IStanleyVariable variable);
	}
}