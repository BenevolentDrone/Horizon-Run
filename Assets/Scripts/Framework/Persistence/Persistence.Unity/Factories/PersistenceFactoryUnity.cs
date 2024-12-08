using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
	public static class PersistenceFactoryUnity
	{
		#region Serialization strategies

		public static PlayerPrefsStrategy BuildPlayerPrefsStrategy(
			string keyPrefs,
			ILoggerResolver loggerResolver = null)
		{
			return new PlayerPrefsStrategy(
				keyPrefs,
				loggerResolver?.GetLogger<PlayerPrefsStrategy>());
		}

		#endregion
	}
}