namespace HereticalSolutions.StanleyScript
{
	public interface IExecutable
	{
		EExecutionStatus Status { get; }

		int CurrentLine { get; }

		int ProgramCounter { get; }

		string[] Instructions { get; }

		void LoadProgram(
			string[] instructions);

		void Start();

		void Step();

		void Pause();

		void Resume();

		void Stop();
	}
}