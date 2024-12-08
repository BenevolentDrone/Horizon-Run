using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
	public static class PersistenceFactoryUnityEditor
	{
		#region Serialization strategies

		public static EditorPrefsStrategy BuildEditorPrefsStrategy(
			string keyPrefs,
			ILoggerResolver loggerResolver = null)
		{
			return new EditorPrefsStrategy(
				keyPrefs,
				loggerResolver?.GetLogger<EditorPrefsStrategy>());
		}

		public static EditorPrefsWithUUIDStrategy<TUUID> BuildEditorPrefsWithUUIDStrategy<TUUID>(
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID uuid,
			ILoggerResolver loggerResolver = null)
		{
			return new EditorPrefsWithUUIDStrategy<TUUID>(
				keyPrefsSerializedValuesList,
				keyPrefsValuePrefix,
				uuid,
				loggerResolver?.GetLogger<EditorPrefsWithUUIDStrategy<TUUID>>());
		}

		#endregion
	}
}