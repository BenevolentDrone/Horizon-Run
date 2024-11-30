using System;
using System.IO;

namespace HereticalSolutions.Persistence
{
	//WORKS ON WINDOWS AND LINUX ONLY
	[Serializable]
	public class FileAtTempPathSettings
		: IPathSettings
	{
		public string RelativePath;

		public string TempFolder
		{
			get
			{
				return Path.GetTempPath();
			}
		}

		public string FullPath
		{
			get
			{
				return Path
					.Combine(
						TempFolder,
						RelativePath)
					.SanitizePath();
			}
		}
	}
}