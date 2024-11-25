using System;
using System.IO;

using UnityEngine;

namespace HereticalSolutions.Persistence
{
	[Serializable]
	public class FileAtPersistentDataPathSettings
	{
		public string RelativePath;

		public string ApplicationDataFolder
		{
			get
			{
				return Application.persistentDataPath;
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