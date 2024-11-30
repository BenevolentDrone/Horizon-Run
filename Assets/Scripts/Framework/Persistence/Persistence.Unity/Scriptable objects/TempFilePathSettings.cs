using UnityEngine;

namespace HereticalSolutions.Persistence
{
	[CreateAssetMenu(fileName = "TempFilePathSettings", menuName = "Settings/Persistence/Temp path settings", order = 2)]
	public class TempFilePathSettings
		: ScriptableObject,
		  IPathSettings
	{
		[SerializeField]
		public FileAtTempPathSettings Settings;

		#region IPathSettings

		public string FullPath { get => Settings.FullPath; }

		#endregion
	}
}