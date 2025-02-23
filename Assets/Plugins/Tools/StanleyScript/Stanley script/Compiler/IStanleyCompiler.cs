namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyCompiler
	{
		string[] ProgramListing { get; }

		void LoadProgram(
			string[] program);

		bool Compile(
			out string[] instructions);

		bool CompileImmediately(
			string programLine,
			out string[] instructions);
	}
}