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


		private Action deferredBuildFormatSerializerDelegate;

		private Action deferredBuildDataConverterDelegate;

		private Action deferredBuildSerializationStrategyDelegate;

		public SerializerBuilder(
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.loggerResolver = loggerResolver;

			this.logger = logger;


			serializerContext = null;


			deferredBuildFormatSerializerDelegate = null;

			deferredBuildDataConverterDelegate = null;

			deferredBuildSerializationStrategyDelegate = null;
		}

		#region ISerializerBuilderInternal

		public ISerializerContext SerializerContext
		{
			get => serializerContext;
			set => serializerContext = value;
		}

		public Action DeferredBuildFormatSerializerDelegate
		{
			get => deferredBuildFormatSerializerDelegate;
			set => deferredBuildFormatSerializerDelegate = value;
		}

		public Action DeferredBuildDataConverterDelegate
		{
			get => deferredBuildDataConverterDelegate;
			set => deferredBuildDataConverterDelegate = value;
		}

		public Action DeferredBuildSerializationStrategyDelegate
		{
			get => deferredBuildSerializationStrategyDelegate;
			set => deferredBuildSerializationStrategyDelegate = value;
		}

		public ILoggerResolver LoggerResolver => loggerResolver;

		public ILogger Logger => logger;

		public void EnsureArgumentsExist()
		{
			if (serializerContext.Arguments == null)
				serializerContext.Arguments =
					MetadataFactory.BuildStronglyTypedMetadata();
		}

		#endregion

		#region ISerializerBuilder

		public ISerializerBuilder NewSerializer()
		{
			serializerContext = new SerializerContext();

			return this;
		}

		public ISerializerBuilder RecycleSerializer(
			ISerializer serializer)
		{
			serializerContext = serializer.Context as ISerializerContext;

			return this;
		}

		#region Settings

		public ISerializerBuilder FromAbsolutePath(
			FileAtAbsolutePathSettings filePathSettings)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {serializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder FromRelativePath(
			FileAtRelativePathSettings filePathSettings)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {serializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder FromTempPath(
			FileAtTempPathSettings filePathSettings)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					filePathSettings.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {serializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder From<TPathSettings>(
			TPathSettings pathSettings)
			where TPathSettings : IPathSettings
		{
			EnsureArgumentsExist();

			var pathSettingsCasted = pathSettings as IPathSettings;

			if (!serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					pathSettingsCasted.FullPath)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {serializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder ErasePath()
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryRemove<IPathArgument>();

			return this;
		}

		#endregion

		#region Format serializer

		public ISerializerBuilder ToObject()
		{
			if (deferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildFormatSerializerDelegate = () =>
			{
				if (serializerContext.FormatSerializer != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				serializerContext.FormatSerializer =
					PersistenceFactory.BuildObjectSerializer(
						loggerResolver);
			};

			return this;
		}

		public ISerializerBuilder ToBinary()
		{
			if (deferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildFormatSerializerDelegate = () =>
			{
				if (serializerContext.FormatSerializer != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				serializerContext.FormatSerializer =
					PersistenceFactory.BuildBinaryFormatterSerializer(
						loggerResolver);
			};

			return this;
		}

		public ISerializerBuilder To<TFormatSerializer>(
			object[] arguments)
			where TFormatSerializer : IFormatSerializer
		{
			if (deferredBuildFormatSerializerDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FORMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildFormatSerializerDelegate = () =>
			{
				if (serializerContext.FormatSerializer != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"FORNMAT SERIALIZER IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				serializerContext.FormatSerializer =
					PersistenceFactory.BuildFormatSerializer<TFormatSerializer>(
						loggerResolver,
						arguments);
			};

			return this;
		}

		public ISerializerBuilder EraseFormatSerializer()
		{
			serializerContext.FormatSerializer = null;

			return this;
		}

		#endregion

		#region Data converters

		public ISerializerBuilder WithLZ4Compression()
		{
			throw new NotImplementedException();
		}

		public ISerializerBuilder WithByteArrayFallback()
		{
			deferredBuildDataConverterDelegate += () =>
			{
				if (serializerContext.DataConverter == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"DATA CONVERTER IS NULL"));
				}

				serializerContext.DataConverter =
					PersistenceFactory.BuildByteArrayFallbackConverter(
						serializerContext.DataConverter,
						null,
						null,
						loggerResolver);
			};

			return this;
		}

		public ISerializerBuilder WithDataConverter<TDataConverter>(
			object[] arguments)
			where TDataConverter : IDataConverter
		{
			deferredBuildDataConverterDelegate += () =>
			{
				if (serializerContext.DataConverter == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"DATA CONVERTER IS NULL"));
				}

				serializerContext.DataConverter =
					PersistenceFactory.BuildDataConverter<TDataConverter>(
						serializerContext.DataConverter,
						loggerResolver,
						arguments);
			};

			return this;
		}

		public ISerializerBuilder EraseDataConverters()
		{
			serializerContext.DataConverter = null;

			return this;
		}

		#endregion

		#region Serialization strategies

		public ISerializerBuilder AsString(
			Func<string> valueGetter,
			Action<string> valueSetter)
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildStringStrategy(
						valueGetter,
						valueSetter,
						loggerResolver);
			};

			return this;
		}

		public ISerializerBuilder AsTextFile()
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!serializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildTextFileStrategy(
						serializerContext.Arguments.Get<IPathArgument>().Path,
						loggerResolver);
			};

			return this;
		}

		public ISerializerBuilder AsBinaryFile()
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!serializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildBinaryFileStrategy(
						serializerContext.Arguments.Get<IPathArgument>().Path,
						loggerResolver);
			};

			return this;
		}

		public ISerializerBuilder AsTextStream(
			bool flushAutomatically = true)
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!serializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildTextStreamStrategy(
						serializerContext.Arguments.Get<IPathArgument>().Path,
						loggerResolver,
						flushAutomatically);
			};

			return this;
		}

		public ISerializerBuilder AsFileStream(
			bool flushAutomatically = true)
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!serializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildFileStreamStrategy(
						serializerContext.Arguments.Get<IPathArgument>().Path,
						loggerResolver,
						flushAutomatically);
			};

			return this;
		}

		public ISerializerBuilder AsMemoryStream(
			byte[] buffer = null,
			int index = -1,
			int count = -1)
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildMemoryStreamStrategy(
						loggerResolver,
						buffer,
						index,
						count);
			};

			return this;
		}

		public ISerializerBuilder AsIsolatedStorageFileStream(
			bool flushAutomatically = true)
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				if (!serializerContext.Arguments.Has<IPathArgument>())
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"PATH ARGUMENT MISSING"));
				}
	
				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildIsolatedStorageStrategy(
						serializerContext.Arguments.Get<IPathArgument>().Path,
						loggerResolver,
						flushAutomatically);
			};

			return this;
		}

		public ISerializerBuilder As<TSerializationStrategy>(
			object[] arguments)
			where TSerializationStrategy : ISerializationStrategy
		{
			if (deferredBuildSerializationStrategyDelegate != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			deferredBuildSerializationStrategyDelegate = () =>
			{
				if (serializerContext.SerializationStrategy != null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"SERIALIZATION STRATEGY IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
				}

				serializerContext.SerializationStrategy =
					PersistenceFactory.BuildSerializationStrategy<TSerializationStrategy>(
						loggerResolver,
						arguments);
			};

			return this;
		}

		public ISerializerBuilder EraseStrategy()
		{
			serializerContext.SerializationStrategy = null;

			return this;
		}

		#endregion

		#region Arguments

		public ISerializerBuilder WithPath(
			string path)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<IPathArgument>(
				PersistenceFactory.BuildPathArgument(
					path)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"PATH ARGUMENT IS ALREADY PRESENT: {serializerContext.Arguments.Get<IPathArgument>().Path}. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder WithAppend()
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<IAppendArgument>(
				PersistenceFactory.BuildAppendArgument()))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"APPEND ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder WithReadWriteAccess()
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<IReadAndWriteAccessArgument>(
				PersistenceFactory.BuildReadAndWriteAccessArgument()))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"READ WRITE ACCESS ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder WithBlockSerialization(
			int blockSize = 1024)
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<IBlockSerializationArgument>(
				PersistenceFactory.BuildBlockSerializationArgument(
					blockSize)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"BLOCK SERIALIZATION IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder With<TInterface, TArgument>(
			object[] arguments)
			where TArgument : TInterface
		{
			EnsureArgumentsExist();

			if (!serializerContext.Arguments.TryAdd<TInterface>(
				PersistenceFactory.BuildSerializationArgument<TArgument>(
					loggerResolver,
					arguments)))
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"{nameof(TInterface)} ARGUMENT IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			return this;
		}

		public ISerializerBuilder Without<TInterface>()
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.TryRemove<TInterface>();

			return this;
		}

		public ISerializerBuilder EraseArguments()
		{
			EnsureArgumentsExist();

			serializerContext.Arguments.Clear();

			return this;
		}

		#endregion

		#region Visitor

		public ISerializerBuilder WithVisitor(
			IVisitor visitor)
		{
			if (serializerContext.Visitor != null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"VISITOR IS ALREADY PRESENT. PLEASE REMOVE IT BEFORE ADDING A NEW ONE"));
			}

			serializerContext.Visitor = visitor;

			return this;
		}

		public ISerializerBuilder EraseVisitor()
		{
			serializerContext.Visitor = null;

			return this;
		}

		#endregion

		#region Build

		public ISerializer BuildSerializer()
		{
			AssembleContext();
			
			return new Serializer(
				serializerContext,
				loggerResolver?.GetLogger<Serializer>());
		}

		public IBlockSerializer BuildBlockSerializer()
		{
			AssembleContext();

			throw new NotImplementedException();
		}

		public IAsyncSerializer BuildAsyncSerializer()
		{
			AssembleContext();

			throw new NotImplementedException();
		}

		public IAsyncBlockSerializer BuildAsyncBlockSerializer()
		{
			AssembleContext();

			throw new NotImplementedException();
		}

		#endregion

		public void Refurbish()
		{
			AssembleContext();
		}

		#endregion

		private void AssembleContext()
		{
			if (serializerContext == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"SERIALIZER CONTEXT MISSING. PLEASE SPECIFY A SERIALIZER CONTEXT BEFORE BUILDING"));
			}

			EnsureArgumentsExist();

			if (deferredBuildFormatSerializerDelegate != null)
			{
				deferredBuildFormatSerializerDelegate();

				deferredBuildFormatSerializerDelegate = null;
			}

			if (serializerContext.FormatSerializer == null)
			{
				serializerContext.FormatSerializer =
					PersistenceFactory.BuildObjectSerializer(
						loggerResolver);
			}

			if (deferredBuildDataConverterDelegate != null)
			{
				serializerContext.DataConverter =
					PersistenceFactory.BuildInvokeStrategyConverter(
						loggerResolver);

				deferredBuildDataConverterDelegate();

				deferredBuildDataConverterDelegate = null;

				if (serializerContext.DataConverter.GetType()
					!= typeof(ByteArrayFallbackConverter))
				{
					serializerContext.DataConverter =
						PersistenceFactory.BuildByteArrayFallbackConverter(
							serializerContext.DataConverter,
							null,
							null,
							loggerResolver);
				}
			}

			if (serializerContext.DataConverter == null)
			{
				serializerContext.DataConverter =
					PersistenceFactory.BuildInvokeStrategyConverter(
						loggerResolver);

				serializerContext.DataConverter =
					PersistenceFactory.BuildByteArrayFallbackConverter(
						serializerContext.DataConverter,
						null,
						null,
						loggerResolver);
			}

			if (serializerContext.SerializationStrategy == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"SERIALIZATION STRATEGY MISSING. PLEASE SPECIFY A SERIALIZATION STRATEGY BEFORE BUILDING"));
			}

			if (serializerContext.Visitor == null)
			{
				serializerContext.Visitor = PersistenceFactory.BuildDispatchVisitor(
					loggerResolver);
			}
		}
	}
}