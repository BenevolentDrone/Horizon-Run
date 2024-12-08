#define CSV_SUPPORT
#define YAML_SUPPORT
#define PROTOBUF_SUPPORT

using System;

using HereticalSolutions.Metadata.Factories;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class SerializerBuilder
		: ISerializerBuilder,
		  ISerializerBuilderInternal
	{
		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		private ISerializerContext serializerContext;

		public SerializerBuilder(
			ILoggerResolver loggerResolver = null,
			ILogger logger = null)
		{
			this.loggerResolver = loggerResolver;

			this.logger = logger;

			serializerContext = null;
		}

		#region ISerializerBuilderInternal

		public ISerializerContext SerializerContext
		{
			get => serializerContext;
			set => serializerContext = value;
		}

		public ILoggerResolver LoggerResolver => loggerResolver;

		public void EnsureArgumentsExist()
		{
			if (serializerContext.Arguments == null)
				serializerContext.Arguments = MetadataFactory.BuildStronglyTypedMetadata();
		}

		#endregion

		#region ISerializerBuilder

		public ISerializerBuilder NewSerializer()
		{
			serializerContext = new SerializerContext();

			return this;
		}

		#region Settings

		public ISerializerBuilder FromAbsolutePath(
			FileAtAbsolutePathSettings filePathSettings)
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath));

			return this;
		}

		public ISerializerBuilder FromRelativePath(
			FileAtRelativePathSettings filePathSettings)
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath));

			return this;
		}

		public ISerializerBuilder FromTempPath(
			FileAtTempPathSettings filePathSettings)
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath));

			return this;
		}

		public ISerializerBuilder From<TPathSettings>(
			TPathSettings pathSettings)
			where TPathSettings : IPathSettings
		{
			EnsureArgumentsExist();

			var pathSettingsCasted = pathSettings as IPathSettings;

			serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					pathSettingsCasted.FullPath));

			return this;
		}

		#endregion

		#region Format serializer

		public ISerializerBuilder ToObject()
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildObjectSerializer(
				loggerResolver);

			return this;
		}

		public ISerializerBuilder ToBinary()
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildBinaryFormatterSerializer(
				loggerResolver);

			return this;
		}

		public ISerializerBuilder ToJSON()
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildJSONSerializer(
				loggerResolver);

			return this;
		}

		public ISerializerBuilder ToXML()
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildXMLSerializer(
				loggerResolver);

			return this;
		}

#if CSV_SUPPORT
		public ISerializerBuilder ToCSV(
			bool includeHeader = true)
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildCSVSerializer(
				includeHeader,
				loggerResolver);

			return this;
		}
#endif

#if YAML_SUPPORT
		public ISerializerBuilder ToYAML()
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildYAMLSerializer(
				loggerResolver);

			return this;
		}
#endif

#if PROTOBUF_SUPPORT
		public ISerializerBuilder ToProtobuf()
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildProtobufSerializer(
				loggerResolver);

			return this;
		}
#endif

		public ISerializerBuilder To<TFormatSerializer>(
			object[] arguments)
			where TFormatSerializer : IFormatSerializer
		{
			serializerContext.FormatSerializer = PersistenceFactory.BuildFormatSerializer<TFormatSerializer>(
				arguments,
				loggerResolver);

			return this;
		}

		#endregion

		#region Serialization strategies

		public ISerializerBuilder AsString()
		{
			serializerContext.SerializationStrategy = PersistenceFactory.BuildStringStrategy(
				loggerResolver);

			return this;
		}

		public ISerializerBuilder AsTextFile()
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.Has<IPathArgument>())
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PATH ARGUMENT MISSING. IF IT IS CHAINED LATER, PLEASE MOVE IT UP THE CHAIN BEFORE THE 'AS' CALL"));
			}

			serializerContext.SerializationStrategy = PersistenceFactory.BuildTextFileStrategy(
				serializerContext.Arguments.Get<IPathArgument>().Path,
				loggerResolver);

			return this;
		}

		public ISerializerBuilder AsBinaryFile()
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.Has<IPathArgument>())
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PATH ARGUMENT MISSING. IF IT IS CHAINED LATER, PLEASE MOVE IT UP THE CHAIN BEFORE THE 'AS' CALL"));
			}

			serializerContext.SerializationStrategy = PersistenceFactory.BuildBinaryFileStrategy(
				serializerContext.Arguments.Get<IPathArgument>().Path,
				loggerResolver);

			return this;
		}

		public ISerializerBuilder AsTextStream(
			bool flushAutomatically = true)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.Has<IPathArgument>())
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PATH ARGUMENT MISSING. IF IT IS CHAINED LATER, PLEASE MOVE IT UP THE CHAIN BEFORE THE 'AS' CALL"));
			}

			serializerContext.SerializationStrategy = PersistenceFactory.BuildTextStreamStrategy(
				serializerContext.Arguments.Get<IPathArgument>().Path,
				flushAutomatically,
				loggerResolver);

			return this;
		}

		public ISerializerBuilder AsFileStream(
			bool flushAutomatically = true)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.Has<IPathArgument>())
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PATH ARGUMENT MISSING. IF IT IS CHAINED LATER, PLEASE MOVE IT UP THE CHAIN BEFORE THE 'AS' CALL"));
			}

			serializerContext.SerializationStrategy = PersistenceFactory.BuildFileStreamStrategy(
				serializerContext.Arguments.Get<IPathArgument>().Path,
				flushAutomatically,
				loggerResolver);

			return this;
		}

		public ISerializerBuilder AsMemoryStream(
			byte[] buffer = null,
			int index = -1,
			int count = -1)
		{
			serializerContext.SerializationStrategy = PersistenceFactory.BuildMemoryStreamStrategy(
				buffer,
				index,
				count,
				loggerResolver);

			return this;
		}

		public ISerializerBuilder AsIsolatedStorageFileStream(
			bool flushAutomatically = true)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.Has<IPathArgument>())
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"PATH ARGUMENT MISSING. IF IT IS CHAINED LATER, PLEASE MOVE IT UP THE CHAIN BEFORE THE 'AS' CALL"));
			}

			serializerContext.SerializationStrategy = PersistenceFactory.BuildIsolatedStorageStrategy(
				serializerContext.Arguments.Get<IPathArgument>().Path,
				flushAutomatically,
				loggerResolver);

			return this;
		}

		public ISerializerBuilder As<TSerializationStrategy>(
			object[] arguments)
			where TSerializationStrategy : ISerializationStrategy
		{
			serializerContext.SerializationStrategy = PersistenceFactory.BuildSerializationStrategy<TSerializationStrategy>(
				arguments,
				loggerResolver);

			return this;
		}

		#endregion

		#region Arguments

		public ISerializerBuilder WithPath(
			string path)
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					path));

			return this;
		}

		public ISerializerBuilder WithAppend()
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<IAppendArgument>(
				PersistenceFactory.BuildAppendArgument());

			return this;
		}

		public ISerializerBuilder WithReadWriteAccess()
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<IReadAndWriteAccessArgument>(
				PersistenceFactory.BuildReadAndWriteAccessArgument());

			return this;
		}

		public ISerializerBuilder WithBlockSerialization(
			int blockSize = 1024)
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<IBlockSerializationArgument>(
				PersistenceFactory.BuildBlockSerializationArgument());

			return this;
		}

		public ISerializerBuilder With<TInterface, TArgument>(
			object[] arguments)
			where TArgument : TInterface
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryAdd<TInterface>(
				PersistenceFactory.BuildSerializationArgument<TArgument>(
					arguments,
					loggerResolver));

			return this;
		}

		#endregion

		public ISerializer Build()
		{
			if (serializerContext == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"SERIALIZER CONTEXT MISSING. PLEASE SPECIFY A SERIALIZER CONTEXT BEFORE BUILDING"));
			}

			EnsureArgumentsExist();

			if (serializerContext.FormatSerializer == null)
			{
				serializerContext.FormatSerializer = PersistenceFactory.BuildObjectSerializer(
					loggerResolver);
			}

			if (serializerContext.SerializationStrategy == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"SERIALIZATION STRATEGY MISSING. PLEASE SPECIFY A SERIALIZATION STRATEGY BEFORE BUILDING"));
			}

			serializerContext.Visitor = PersistenceFactory.BuildDispatchVisitor(
				loggerResolver);

			return new Serializer(
				serializerContext,
				loggerResolver?.GetLogger<Serializer>());
		}

		#endregion
	}
}