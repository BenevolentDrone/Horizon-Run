using System;
using System.IO;

namespace HereticalSolutions.Persistence
{
	[Serializable]
	public class FileAtRelativePathSettings
		: IPathSettings
	{
		public string AbsoluteFolderPath; //PRAISE THE ABSOLUTE

		public string RelativePath;

		public string FullPath
		{
			get
			{
				return Path
					.Combine(
						AbsoluteFolderPath,
						RelativePath)
					.SanitizePath();
			}
		}
	}
}