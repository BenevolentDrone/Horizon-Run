using System;
using System.IO;

using UnityEngine;

namespace HereticalSolutions.Persistence
{
	[Serializable]
	public class FileAtApplicationDataPathSettings
	{
		public string RelativePath;

		public string ApplicationDataFolder
		{
			get
			{
				return Application.dataPath;
			}
		}

		public string FullPath
		{
			get
			{
				return Path
					.Combine(
						ApplicationDataFolder,
						RelativePath)
					.SanitizePath();
			}
		}
	}
}