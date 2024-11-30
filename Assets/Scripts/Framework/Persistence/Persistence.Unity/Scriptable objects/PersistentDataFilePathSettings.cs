using UnityEngine;

namespace HereticalSolutions.Persistence
{
	[CreateAssetMenu(fileName = "PersistentDataFilePathSettings", menuName = "Settings/Persistence/Persistent data path settings", order = 2)]
	public class PersistentDataFilePathSettings
		: ScriptableObject,
		  IPathSettings
	{
		[SerializeField]
		public FileAtPersistentDataPathSettings Settings;

		#region IPathSettings

		public string FullPath { get => Settings.FullPath; }

		#endregion
	}
}