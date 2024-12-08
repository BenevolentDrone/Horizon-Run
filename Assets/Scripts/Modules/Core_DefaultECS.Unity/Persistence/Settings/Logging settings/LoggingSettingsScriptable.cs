using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	[CreateAssetMenu(
		fileName = "Logging settings",
		menuName = "Settings/Logging/Logging settings",
		order = 0)]
	public class LoggingSettingsScriptable : ScriptableObject
	{
		[Header("Basic logging settings")]

		public LoggingSettings BasicLoggingSettings;

		[Header("Unity logging settings")]

		public bool SendDebugLogsToLogger = true;
	}
}