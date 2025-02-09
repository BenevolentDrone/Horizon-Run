using System;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public static class SerializerBuilderExtensionsUnity
	{
		public static ISerializerBuilder FromApplicationDataPath(
			this ISerializerBuilder builder,
			FileAtApplicationDataPathSettings filePathSettings)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.EnsureArgumentsExist();

			if (!builderCasted.SerializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					builderCasted.Logger.TryFormatException(
						builderCasted.GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {builderCasted.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return builder;
		}

		public static ISerializerBuilder FromPersistentDataPath(
			this ISerializerBuilder builder,
			FileAtPersistentDataPathSettings filePathSettings)
		{
			var builderCasted = builder as ISerializerBuilderInternal;

			builderCasted.EnsureArgumentsExist();

			if (!builderCasted.SerializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					builderCasted.Logger.TryFormatException(
						builderCasted.GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {builderCasted.SerializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return builder;
		}

		public static ISerializerBuilder AsPlayerPrefs(
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
					PersistenceFactoryUnity.BuildPlayerPrefsStrategy(
						keyPrefs,
						builderCasted.LoggerResolver);
			};

			return builder;
		}
	}
}