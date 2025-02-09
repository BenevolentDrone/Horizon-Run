using System;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public static class PersistenceHelpersUnityEditor
	{
		public static ISerializerBuilder AsEditorPrefs(
			this ISerializerBuilder builder,
			string keyPrefs)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			if (builderCasted.DeferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					builderCasted.Logger.TryFormatException(
						builderCasted.GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			builderCasted.DeferredBuildSerializationStrategyDelegate = () =>
			{
				if (builderCasted.SerializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						builderCasted.Logger.TryFormatException(
							builderCasted.GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!builderCasted.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						builderCasted.Logger.TryFormatException(
							builderCasted.GetType(),
							"PATH ARGUMENT MISSING"));
				}

				builderCasted.SerializerContext.SerializationStrategy =
					PersistenceFactoryUnityEditor.BuildEditorPrefsStrategy(
						keyPrefs,
						builderCasted.LoggerResolver);
			};

			return builder;
		}

		public static ISerializerBuilder AsEditorPrefsWithUUID<TUUID>(
			this ISerializerBuilder builder,
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID uuid)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			if (builderCasted.DeferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					builderCasted.Logger.TryFormatException(
						builderCasted.GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			builderCasted.DeferredBuildSerializationStrategyDelegate = () =>
			{
				if (builderCasted.SerializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						builderCasted.Logger.TryFormatException(
							builderCasted.GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!builderCasted.SerializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						builderCasted.Logger.TryFormatException(
							builderCasted.GetType(),
							"PATH ARGUMENT MISSING"));
				}

				builderCasted.SerializerContext.SerializationStrategy =
					PersistenceFactoryUnityEditor.BuildEditorPrefsWithUUIDStrategy<TUUID>(
						keyPrefsSerializedValuesList,
						keyPrefsValuePrefix,
						uuid,
						builderCasted.LoggerResolver);
			};

			return builder;
		}
	}
}