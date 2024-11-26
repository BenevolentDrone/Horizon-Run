using System;

namespace HereticalSolutions.Persistence
{
	[Serializable]
	public class FileAtAbsolutePathSettings
		: IPathSettings
	{
		public string Path;

		public string FullPath { get => Path; }
	}
}