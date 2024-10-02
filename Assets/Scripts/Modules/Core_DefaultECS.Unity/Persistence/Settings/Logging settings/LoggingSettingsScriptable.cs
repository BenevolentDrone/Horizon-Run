using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	[CreateAssetMenu(fileName = "Logging settings", menuName = "Settings/Logging/Logging settings", order = 0)]
	public class LoggingSettingsScriptable : ScriptableObject
	{
		[Header("Basic logging settings")]

		public LoggingSettings BasicLoggingSettings;

		[Header("Logging environment settings")]

		public LoggingEnvironmentSettings LoggingEnvironmentSettings;

		[Header("Unity logging settings")]

		public bool SendDebugLogsToLogger = true;
	}
}