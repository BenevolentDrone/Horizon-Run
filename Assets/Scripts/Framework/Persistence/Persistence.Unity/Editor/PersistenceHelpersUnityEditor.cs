using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Persistence
{
	public static class PersistenceHelpersUnityEditor
	{
		public static ISerializerBuilder AsEditorPrefs(
			this ISerializerBuilder builder,
			string keyPrefs)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.SerializationStrategy =
				PersistenceFactoryUnityEditor.BuildEditorPrefsStrategy(
					keyPrefs,
					builderCasted.LoggerResolver);

			return builder;
		}

		public static ISerializerBuilder AsEditorPrefsWithUUID<TUUID>(
			this ISerializerBuilder builder,
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID uuid)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.SerializationStrategy =
				PersistenceFactoryUnityEditor.BuildEditorPrefsWithUUIDStrategy<TUUID>(
					keyPrefsSerializedValuesList,
					keyPrefsValuePrefix,
					uuid,
					builderCasted.LoggerResolver);

			return builder;
		}
	}
}