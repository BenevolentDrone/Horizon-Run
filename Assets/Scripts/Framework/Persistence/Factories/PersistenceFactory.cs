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

namespace HereticalSolutions.Persistence.Factories
{
    public static class PersistenceFactory
    {
        #region Visitors

        public static DispatchVisitor BuildDispatchVisitor(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, IEnumerable<IVisitor>> concreteVisitorRepository =
                RepositoriesFactory.BuildDictionaryRepository<Type, IEnumerable<IVisitor>>();

            TypeHelpers.GetTypesWithAttributeInAllAssemblies<VisitorAttribute>(
                out Type[] visitorTypes);

            foreach (Type visitorType in visitorTypes)
            {
                if (Activator.CreateInstance(visitorType) is IVisitor visitor)
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

            return new DispatchVisitor(
                concreteVisitorRepository,
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

        #endregion

        #region Format serializers

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

        #endregion
    }
}