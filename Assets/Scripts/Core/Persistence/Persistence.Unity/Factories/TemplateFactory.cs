using System;

using HereticalSolutions.Persistence.Serializers;
using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence.Factories
{
    public static class UnityPersistenceFactory
    {
        public static BinarySerializer BuildSimpleUnityBinarySerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StreamArgument), new SerializeBinaryIntoStreamStrategy(
                loggerResolver?.GetLogger<SerializeBinaryIntoStreamStrategy>()));
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeBinaryIntoStreamStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new BinarySerializer(
                strategyRepository,
                loggerResolver?.GetLogger<BinarySerializer>());
        }
        
        public static ProtobufSerializer BuildSimpleUnityProtobufSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StreamArgument), new SerializeProtobufIntoStreamStrategy(
                loggerResolver?.GetLogger<SerializeProtobufIntoStreamStrategy>()));
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeProtobufIntoStreamStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new ProtobufSerializer(
                strategyRepository,
                loggerResolver?.GetLogger<ProtobufSerializer>());
        }
        
        public static JSONSerializer BuildSimpleUnityJSONSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeJsonIntoStringStrategy(
                loggerResolver?.GetLogger<SerializeJsonIntoStringStrategy>()));
            
            database.Add(typeof(StreamArgument), new SerializeJsonIntoStreamStrategy(
                loggerResolver?.GetLogger<SerializeJsonIntoStreamStrategy>()));

            database.Add(typeof(TextFileArgument), new SerializeJsonIntoTextFileStrategy(
                loggerResolver?.GetLogger<SerializeJsonIntoTextFileStrategy>()));
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeJsonIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeJsonIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeJsonIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new JSONSerializer(
                strategyRepository,
                loggerResolver?.GetLogger<JSONSerializer>());
        }

        public static XMLSerializer BuildSimpleUnityXMLSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeXmlIntoStringStrategy(
                loggerResolver?.GetLogger<SerializeXmlIntoStringStrategy>()));
            
            database.Add(typeof(StreamArgument), new SerializeXmlIntoStreamStrategy(
                loggerResolver?.GetLogger<SerializeXmlIntoStreamStrategy>()));

            database.Add(typeof(TextFileArgument), new SerializeXmlIntoTextFileStrategy(
                loggerResolver?.GetLogger<SerializeXmlIntoTextFileStrategy>()));
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeXmlIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeXmlIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeXmlIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new XMLSerializer(
                strategyRepository,
                loggerResolver?.GetLogger<XMLSerializer>());
        }
        
        public static YAMLSerializer BuildSimpleUnityYAMLSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeYamlIntoStringStrategy(
                loggerResolver?.GetLogger<SerializeYamlIntoStringStrategy>()));
            
            database.Add(typeof(StreamArgument), new SerializeYamlIntoStreamStrategy(
                loggerResolver?.GetLogger<SerializeYamlIntoStreamStrategy>()));

            database.Add(typeof(TextFileArgument), new SerializeYamlIntoTextFileStrategy(
                loggerResolver?.GetLogger<SerializeYamlIntoTextFileStrategy>()));
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeYamlIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeYamlIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeYamlIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new YAMLSerializer(
                strategyRepository,
                loggerResolver?.GetLogger<YAMLSerializer>());
        }
        
        public static CSVSerializer BuildSimpleUnityCSVSerializer(
            ILoggerResolver loggerResolver = null)
        {
            IRepository<Type, object> database = RepositoriesFactory.BuildDictionaryRepository<Type, object>();
            
            database.Add(typeof(StringArgument), new SerializeCsvIntoStringStrategy(
                loggerResolver?.GetLogger<SerializeCsvIntoStringStrategy>()));
            
            database.Add(typeof(StreamArgument), new SerializeCsvIntoStreamStrategy(
                loggerResolver?.GetLogger<SerializeCsvIntoStreamStrategy>()));

            database.Add(typeof(TextFileArgument), new SerializeCsvIntoTextFileStrategy(
                loggerResolver?.GetLogger<SerializeCsvIntoTextFileStrategy>()));
            
            database.Add(typeof(UnityStreamArgument), new UnitySerializeCsvIntoStreamStrategy());
            database.Add(typeof(UnityTextFileArgument), new UnitySerializeCsvIntoTextFileStrategy());
            
            database.Add(typeof(UnityPlayerPrefsArgument), new UnitySerializeCsvIntoPlayerPrefsStrategy());
            
            IReadOnlyObjectRepository strategyRepository = RepositoriesFactory.BuildDictionaryObjectRepository(database);
            
            return new CSVSerializer(
                strategyRepository,
                loggerResolver?.GetLogger<CSVSerializer>());
        }
    }
}