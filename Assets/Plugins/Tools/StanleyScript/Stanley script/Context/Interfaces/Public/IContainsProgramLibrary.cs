using System.Collections.Generic;

namespace HereticalSolutions.StanleyScript
{
	public interface IContainsProgramLibrary
	{
		string CurrentProgram { get; }

		IEnumerable<string> AllProgramNames { get; }

		bool LoadProgramFromLibrary(
			string name);

		bool GetProgramFromLibrary(
			string name,
			out string[] program);

		bool SaveProgramToLibrary(
			string name,
			string[] program);

		bool SaveCurrentProgramFromLibrary(
			string name);

		bool RemoveProgramFromLibrary(
			string name);
	}
}