using UnityEngine;

namespace HereticalSolutions.Persistence
{
	[CreateAssetMenu(fileName = "ApplicationDataFilePathSettings", menuName = "Settings/Persistence/Application data path settings", order = 2)]
	public class ApplicationDataFilePathSettings
		: ScriptableObject,
		  IPathSettings
	{
		[SerializeField]
		public FileAtApplicationDataPathSettings Settings;

		#region IPathSettings

		public string FullPath { get => Settings.FullPath; }

		#endregion
	}
}