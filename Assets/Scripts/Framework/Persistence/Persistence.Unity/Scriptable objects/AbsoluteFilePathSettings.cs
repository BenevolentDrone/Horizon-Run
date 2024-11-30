using UnityEngine;

namespace HereticalSolutions.Persistence
{
	[CreateAssetMenu(fileName = "AbsoluteFilePathSettings", menuName = "Settings/Persistence/Absolute path settings", order = 2)]
	public class AbsoluteFilePathSettings
		: ScriptableObject,
		  IPathSettings
	{
		[SerializeField]
		public FileAtAbsolutePathSettings Settings;

		#region IPathSettings

		public string FullPath { get => Settings.FullPath; }

		#endregion
	}
}