namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyContextInternal
		: IStanleyContext,
		  IContainsContextHierarchy
	{
		bool EmitShortcutInstructions { get; }

		bool LogAllExecutedCommands { get; }

		void Clear();
	}
}