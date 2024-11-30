using UnityEngine;

namespace HereticalSolutions.Persistence
{
	[CreateAssetMenu(fileName = "RelativeFilePathSettings", menuName = "Settings/Persistence/Relative path settings", order = 2)]
	public class RelativeFilePathSettings
		: ScriptableObject,
		  IPathSettings
	{
		[SerializeField]
		public FileAtRelativePathSettings Settings;

		#region IPathSettings

		public string FullPath { get => Settings.FullPath; }

		#endregion
	}
}