using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Persistence
{
	public static class PersistenceHelpersUnity
	{
		public static ISerializerBuilder FromApplicationDataPath(
			this ISerializerBuilder builder,
			FileAtApplicationDataPathSettings filePathSettings)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath));

			return builder;
		}

		public static ISerializerBuilder FromPersistentDataPath(
			this ISerializerBuilder builder,
			FileAtPersistentDataPathSettings filePathSettings)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath));

			return builder;
		}

		public static ISerializerBuilder AsPlayerPrefs(
			this ISerializerBuilder builder,
			string keyPrefs)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.SerializerContext.SerializationStrategy = 
				PersistenceFactoryUnity.BuildPlayerPrefsStrategy(
					keyPrefs,
					builderCasted.LoggerResolver);

			return builder;
		}
	}
}