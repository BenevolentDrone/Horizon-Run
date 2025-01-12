#define CSV_SUPPORT
#define YAML_SUPPORT
#define PROTOBUF_SUPPORT

#if YAML_SUPPORT
using YamlDotNet.Serialization;

using YamlSerializerBuilder = YamlDotNet.Serialization.SerializerBuilder;
using YamlDeserializerBuilder = YamlDotNet.Serialization.DeserializerBuilder;

using YamlDotNetSerializer = YamlDotNet.Serialization.ISerializer;
using YamlDotNetDeserializer = YamlDotNet.Serialization.IDeserializer;
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;
using System.Linq;

namespace HereticalSolutions.Persistence.Factories
{
    public static class PersistenceFactory
    {
        private static readonly Type[] formatSerializerTypes;

        private static readonly Type[] serializationArgumentTypes;

        private static readonly Type[] serializationStrategyTypes;

        private static readonly IRepository<Type, IEnumerable<IVisitor>> visitorRepository;

        #region Initialization

        static PersistenceFactory()
        {
            TypeHelpers.GetTypesWithAttributeInAllAssemblies<FormatSerializerAttribute>(
                out Type[] formatSerializerTypes);

            TypeHelpers.GetTypesWithAttributeInAllAssemblies<SerializationArgumentAttribute>(
                out Type[] serializationArgumentTypes);

            TypeHelpers.GetTypesWithAttributeInAllAssemblies<SerializationStrategyAttribute>(
                out Type[] serializationStrategyTypes);

            visitorRepository = CreateVisitorRepository();
        }

        private static IRepository<Type, IEnumerable<IVisitor>> CreateVisitorRepository()
        {
            IRepository<Type, IEnumerable<IVisitor>> concreteVisitorRepository =
                RepositoriesFactory.BuildDictionaryRepository<Type, IEnumerable<IVisitor>>();

            TypeHelpers.GetTypesWithAttributeInAllAssemblies<VisitorAttribute>(
                out Type[] visitorTypes);


            foreach (Type visitorType in visitorTypes)
            {
                try
                {
                    object instance = null;

                    var hasDefaultConstructor = visitorType.GetConstructor(Type.EmptyTypes) != null;

                    if (hasDefaultConstructor)
                    {
                        instance = Activator.CreateInstance(visitorType);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(visitorType,
                            BindingFlags.CreateInstance |
                            BindingFlags.Public |
                            BindingFlags.Instance |
                            BindingFlags.OptionalParamBinding,
                            null, new Object[] { Type.Missing }, null);
                    }

                    if (instance == null)
                        continue;

                    if (instance is IVisitor visitor)
                    {
                        var attribute = visitorType.GetCustomAttribute<VisitorAttribute>(false);
    
                        var visitableType = attribute.TargetType;
    
                        if (concreteVisitorRepository.Has(visitableType))
                        {
                            var visitorList = concreteVisitorRepository
                                .Get(visitableType)
                                as List<IVisitor>;
    
                            visitorList.Add(visitor);
                        }
                        else
                        {
                            concreteVisitorRepository.Add(
                                visitableType,
                                new List<IVisitor> { visitor });
                        }
                    }
                }
                catch (MissingMethodException methodMissingException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    throw new Exception(
                        $"FAILED TO CREATE VISITOR INSTANCE FOR TYPE {visitorType.Name}",
                        e);
                }
            }

            return concreteVisitorRepository;
        }

        #endregion

        #region Visitors

        public static DispatchVisitor BuildDispatchVisitor(
            ILoggerResolver loggerResolver = null)
        {
            return new DispatchVisitor(
                visitorRepository,
                loggerResolver?.GetLogger<DispatchVisitor>());
        }

        #endregion

        #region Arguments

        public static AppendArgument BuildAppendArgument()
        {
            return new AppendArgument
            {
            };
        }

        public static PathArgument BuildPathArgument(
            string path)
        {
            return new PathArgument
            {
                Path = path
            };
        }

        public static BlockSerializationArgument BuildBlockSerializationArgument(
            int blockSize = 1024)
        {
            return new BlockSerializationArgument
            {
                BlockSize = blockSize
            };
        }

        public static ReadAndWriteAccessArgument BuildReadAndWriteAccessArgument()
        {
            return new ReadAndWriteAccessArgument
            {
            };
        }

        public static TSerializationArgument BuildSerializationArgument<TSerializationArgument>(
            object[] arguments = null,
            ILoggerResolver loggerResolver = null)
        {
            if (Array.IndexOf(
                serializationArgumentTypes,
                typeof(TSerializationArgument))
                == -1) // to avoid using linq's Contains
            {
                throw new Exception(
                    $"TYPE {nameof(TSerializationArgument)} IS NOT A VALID SERIALIZATION ARGUMENT TYPE");
            }

            arguments = TryAppendLogger(
                typeof(TSerializationArgument),
                arguments,
                loggerResolver);

            return (TSerializationArgument)Activator.CreateInstance(
                typeof(TSerializationArgument),
                 arguments);
        }

        #endregion

        #region Format serializers

        public static ObjectSerializer BuildObjectSerializer(
            ILoggerResolver loggerResolver = null)
        {
            return new ObjectSerializer(
                loggerResolver?.GetLogger<ObjectSerializer>());
        }

        public static BinaryFormatterSerializer BuildBinaryFormatterSerializer(
            ILoggerResolver loggerResolver = null)
        {
            return new BinaryFormatterSerializer(
                new BinaryFormatter(),
                loggerResolver?.GetLogger<BinaryFormatterSerializer>());
        }

        public static JSONSerializer BuildJSONSerializer(
            ILoggerResolver loggerResolver = null)
        {
            return new JSONSerializer(
                loggerResolver?.GetLogger<JSONSerializer>());
        }

        public static XMLSerializer BuildXMLSerializer(
            ILoggerResolver loggerResolver = null)
        {
            return new XMLSerializer(
                loggerResolver?.GetLogger<XMLSerializer>());
        }

#if CSV_SUPPORT
        public static CSVSerializer BuildCSVSerializer(
            bool includeHeader,
            ILoggerResolver loggerResolver = null)
        {
            return new CSVSerializer(
                includeHeader,
                loggerResolver?.GetLogger<CSVSerializer>());
        }
#endif

#if YAML_SUPPORT
        public static YAMLSerializer BuildYAMLSerializer(
            ILoggerResolver loggerResolver = null)
        {
            return new YAMLSerializer(
                new YamlSerializerBuilder().Build(),
                new YamlDeserializerBuilder().Build(),
                loggerResolver?.GetLogger<YAMLSerializer>());
        }
#endif

#if PROTOBUF_SUPPORT
        public static ProtobufSerializer BuildProtobufSerializer(
            ILoggerResolver loggerResolver = null)
        {
            return new ProtobufSerializer(
                loggerResolver?.GetLogger<ProtobufSerializer>());
        }
#endif

        public static TFormatSerializer BuildFormatSerializer<TFormatSerializer>(
            object[] arguments = null,
            ILoggerResolver loggerResolver = null)
            where TFormatSerializer : IFormatSerializer
        {
            if (Array.IndexOf(
                formatSerializerTypes,
                typeof(TFormatSerializer))
                == -1) // to avoid using linq's Contains
            {
                throw new Exception(
                    $"TYPE {nameof(TFormatSerializer)} IS NOT A VALID FORMAT SERIALIZER TYPE");
            }

            arguments = TryAppendLogger(
                typeof(TFormatSerializer),
                arguments,
                loggerResolver);

            return (TFormatSerializer)Activator.CreateInstance(
                typeof(TFormatSerializer),
                 arguments);
        }

        #endregion

        #region Serialization strategies

        public static StringStrategy BuildStringStrategy(
            ILoggerResolver loggerResolver = null)
        {
            return new StringStrategy(
                loggerResolver?.GetLogger<StringStrategy>());
        }

        public static TextFileStrategy BuildTextFileStrategy(
            string fullPath,
            ILoggerResolver loggerResolver = null)
        {
            return new TextFileStrategy(
                fullPath,
                loggerResolver?.GetLogger<TextFileStrategy>());
        }

        public static BinaryFileStrategy BuildBinaryFileStrategy(
            string fullPath,
            ILoggerResolver loggerResolver = null)
        {
            return new BinaryFileStrategy(
                fullPath,
                loggerResolver?.GetLogger<BinaryFileStrategy>());
        }

        public static TextStreamStrategy BuildTextStreamStrategy(
            string fullPath,
            bool flushAutomatically = true,
            ILoggerResolver loggerResolver = null)
        {
            return new TextStreamStrategy(
                fullPath,
                flushAutomatically,
                loggerResolver?.GetLogger<TextStreamStrategy>());
        }

        public static FileStreamStrategy BuildFileStreamStrategy(
            string fullPath,
            bool flushAutomatically = true,
            ILoggerResolver loggerResolver = null)
        {
            return new FileStreamStrategy(
                fullPath,
                flushAutomatically,
                loggerResolver?.GetLogger<FileStreamStrategy>());
        }

        public static MemoryStreamStrategy BuildMemoryStreamStrategy(
            byte[] buffer = null,
            int index = -1,
            int count = -1,
            ILoggerResolver loggerResolver = null)
        {
            return new MemoryStreamStrategy(
                buffer,
                index,
                count,
                loggerResolver?.GetLogger<MemoryStreamStrategy>());
        }

        public static IsolatedStorageStrategy BuildIsolatedStorageStrategy(
            string fullPath,
            bool flushAutomatically = true,
            ILoggerResolver loggerResolver = null)
        {
            return new IsolatedStorageStrategy(
                fullPath,
                flushAutomatically,
                loggerResolver?.GetLogger<IsolatedStorageStrategy>());
        }

        public static TSerializationStrategy BuildSerializationStrategy<TSerializationStrategy>(
            object[] arguments = null,
            ILoggerResolver loggerResolver = null)
            where TSerializationStrategy : ISerializationStrategy
        {
            if (Array.IndexOf(
                serializationStrategyTypes,
                typeof(TSerializationStrategy))
                == -1) // to avoid using linq's Contains
            {
                throw new Exception(
                    $"TYPE {nameof(TSerializationStrategy)} IS NOT A VALID SERIALIZATION STRATEGY TYPE");
            }

            arguments = TryAppendLogger(
                typeof(TSerializationStrategy),
                arguments,
                loggerResolver);

            return (TSerializationStrategy)Activator.CreateInstance(
                typeof(TSerializationStrategy),
                 arguments);
        }

        #endregion

        #region Builder

        public static SerializerBuilder BuildSerializerBuilder(
            ILoggerResolver loggerResolver = null)
        {
            return new SerializerBuilder(
                loggerResolver,
                loggerResolver?.GetLogger<SerializerBuilder>());
        }

        #endregion

        private static object[] TryAppendLogger(
            Type type,
            object[] arguments,
            ILoggerResolver loggerResolver)
        {
            if (loggerResolver == null)
            {
                return arguments;
            }

            var constructor = type.GetConstructors()[0];

            var parameters = constructor.GetParameters();

            if (parameters.Length == 0)
            {
                return arguments;
            }

            bool canReceiveLogger = parameters[parameters.Length - 1].ParameterType.IsAssignableFrom(typeof(ILogger));

            if (!canReceiveLogger)
            {
                return arguments;
            }

            List<object> argumentList = new List<object>(arguments);

            argumentList.Add(
                loggerResolver.GetLogger(type));

            return argumentList.ToArray();
        }
    }
}