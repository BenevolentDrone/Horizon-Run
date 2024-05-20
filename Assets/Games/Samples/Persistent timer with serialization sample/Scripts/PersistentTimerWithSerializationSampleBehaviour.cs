using System;

using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.Factories;
using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.PersistentTimerWithSerializationSample
{
	public class PersistentTimerWithSerializationSampleBehaviour : MonoBehaviour
	{
		[SerializeField]
		private UnityPersistentFilePathSettings binFSSettings;

		[SerializeField]
		private UnityPersistentFilePathSettings protoFSSettings;

		[SerializeField]
		private UnityPersistentFilePathSettings jsonFSSettings;

		[SerializeField]
		private UnityPersistentFilePathSettings xmlFSSettings;

		[SerializeField]
		private UnityPersistentFilePathSettings yamlFSSettings;

		[SerializeField]
		private UnityPersistentFilePathSettings csvFSSettings;

		[SerializeField]
		private float autosaveCooldown = 5f;

		[SerializeField]
		private float debugCountdown;

		[SerializeField]
		private bool append = false;

		[SerializeField]
		private float forceDeserializationRoll = -1f;

		//Timers
		private IPersistentTimer persistentTimer;

		private IVisitable persistentTimerAsVisitable;

		private ITickable persistentTimerAsTickable;

		//Visitors
		private ISaveVisitor saveVisitor;

		private ILoadVisitor loadVisitor;

		//Serializers
		private ISerializer binarySerializer;

		private ISerializer protobufSerializer;

		private ISerializer jsonSerializer;

		private ISerializer xmlSerializer;

		private ISerializer yamlSerializer;

		private ISerializer csvSerializer;

		//Arguments
		private UnityStreamArgument binaryStreamArgument;

		private UnityStreamArgument protobufStreamArgument;

		private UnityTextFileArgument jsonTextFileArgument;

		private UnityTextFileArgument xmlTextFileArgument;

		private UnityTextFileArgument yamlTextFileArgument;

		private UnityTextFileArgument csvTextFileArgument;

		//Countdowns
		private float countdown;

		//Loggers
		private ILoggerResolver loggerResolver;

		private ILogger logger;

		void Start()
		{
			#region Initiate logger resolver and logger itself

			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerBuilder
				.ToggleAllowedByDefault(false)
				.AddOrWrap(
					LoggersFactoryUnity.BuildUnityDebugLogger())
				.AddOrWrap(
					LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
						loggerBuilder.CurrentLogger))
				.AddOrWrap(
					LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
						loggerBuilder.CurrentLogger))
				.ToggleLogSource(typeof(PersistentTimerWithSerializationSampleBehaviour), true);

			loggerResolver = (ILoggerResolver)loggerBuilder;

			logger = loggerResolver.GetLogger<PersistentTimerWithSerializationSampleBehaviour>();

			#endregion

			//Initialize timers
			persistentTimer = TimeFactory.BuildPersistentTimer(
				"AccumulatingPersistentTimer",
				default(TimeSpan),
				loggerResolver);

			persistentTimer.Accumulate = true;

			persistentTimerAsVisitable = (IVisitable)persistentTimer;

			persistentTimerAsTickable = (ITickable)persistentTimer;

			//Initialize visitors
			var visitor = TimeFactory.BuildSimpleCompositeVisitorWithTimerVisitors(loggerResolver);

			saveVisitor = visitor;

			loadVisitor = visitor;

			//Initialize serializers
			binarySerializer = UnityPersistenceFactory.BuildSimpleUnityBinarySerializer(loggerResolver);

			protobufSerializer = UnityPersistenceFactory.BuildSimpleUnityProtobufSerializer(loggerResolver);

			jsonSerializer = UnityPersistenceFactory.BuildSimpleUnityJSONSerializer(loggerResolver);

			xmlSerializer = UnityPersistenceFactory.BuildSimpleUnityXMLSerializer(loggerResolver);

			yamlSerializer = UnityPersistenceFactory.BuildSimpleUnityYAMLSerializer(loggerResolver);

			csvSerializer = UnityPersistenceFactory.BuildSimpleUnityCSVSerializer(loggerResolver);

			//Initialize arguments
			binaryStreamArgument = new UnityStreamArgument();

			binaryStreamArgument.Settings = binFSSettings;

			protobufStreamArgument = new UnityStreamArgument();

			protobufStreamArgument.Settings = protoFSSettings;

			jsonTextFileArgument = new UnityTextFileArgument();

			jsonTextFileArgument.Settings = jsonFSSettings;

			xmlTextFileArgument = new UnityTextFileArgument();

			xmlTextFileArgument.Settings = xmlFSSettings;

			yamlTextFileArgument = new UnityTextFileArgument();

			yamlTextFileArgument.Settings = yamlFSSettings;

			csvTextFileArgument = new UnityTextFileArgument();

			csvTextFileArgument.Settings = csvFSSettings;

			//Initialize countdown
			countdown = autosaveCooldown;


			if (append)
			{
				//Deserialize
				if (!Load())
				{
					//Start timers
					persistentTimer.Start();

					//Serialize
					Save();
				}
			}
			else
			{
				//Start timers
				persistentTimer.Start();

				//Serialize
				Save();
			}
		}

		void Update()
		{
			persistentTimerAsTickable.Tick(UnityEngine.Time.deltaTime);

			countdown -= UnityEngine.Time.deltaTime;

			if (countdown < 0f)
			{
				countdown = autosaveCooldown;

				Save();
			}

			debugCountdown = countdown;
		}

		private void Save()
		{
			//Visit
			persistentTimerAsVisitable.Accept(saveVisitor, out var dto);

			//Serialize
			binarySerializer.Serialize(binaryStreamArgument, persistentTimerAsVisitable.DTOType, dto);

			//Skip for DTOs with no attributes defined
			//protobufSerializer.Serialize(protobufStreamArgument, persistentTimerAsVisitable.DTOType, dto);

			jsonSerializer.Serialize(jsonTextFileArgument, persistentTimerAsVisitable.DTOType, dto);

			xmlSerializer.Serialize(xmlTextFileArgument, persistentTimerAsVisitable.DTOType, dto);

			yamlSerializer.Serialize(yamlTextFileArgument, persistentTimerAsVisitable.DTOType, dto);

			//Skip for DTOs with no attributes defined
			//csvSerializer.Serialize(csvTextFileArgument, persistentTimerAsVisitable.DTOType, dto);


			//Debug
			var timeProgress = ((IPersistentTimerContext)persistentTimer).SavedProgress;

			logger?.Log<PersistentTimerWithSerializationSampleBehaviour>(
				$"ACCUMULATING PERSISTENT TIMER SERIALIZED. PROGRESS: HOURS: {timeProgress.Hours.ToString()} MINUTES: {timeProgress.Minutes.ToString()} SECONDS: {timeProgress.Seconds.ToString()}");
		}

		private bool Load()
		{
			object dto;

			//Roll deserialization method
			float roll = UnityEngine.Random.Range(0f, 1f);

			if (forceDeserializationRoll > 0f)
				roll = forceDeserializationRoll;

			bool deserialized;

			if (roll < 0.16f) //BINARY
				deserialized = binarySerializer.Deserialize(binaryStreamArgument, persistentTimerAsVisitable.DTOType, out dto);
			else if (roll < 0.33f) //PROTOBUF
			{
				//Skip for DTOs with no attributes defined
				//deserialized = protobufSerializer.Deserialize(protobufStreamArgument, persistentTimerAsVisitable.DTOType,  out dto);

				return false;
			}
			else if (roll < 0.5f) //JSON
				deserialized = jsonSerializer.Deserialize(jsonTextFileArgument, persistentTimerAsVisitable.DTOType, out dto);
			else if (roll < 0.66f) //XML
				deserialized = xmlSerializer.Deserialize(xmlTextFileArgument, persistentTimerAsVisitable.DTOType, out dto);
			else if (roll < 0.83f) //YAML
				deserialized = yamlSerializer.Deserialize(yamlTextFileArgument, persistentTimerAsVisitable.DTOType, out dto);
			else //CSV
			{
				//Skip for DTOs with no attributes defined
				//deserialized = csvSerializer.Deserialize(csvTextFileArgument, persistentTimerAsVisitable.DTOType,  out dto);

				return false;
			}

			if (!deserialized)
				return false;

			//Visit
			bool result = persistentTimerAsVisitable.Accept(loadVisitor, dto);

			//Debug
			if (result)
			{
				var timeProgress = ((IPersistentTimerContext)persistentTimer).SavedProgress;

				string methodRolled = string.Empty;

				if (roll < 0.16f) //BINARY
					methodRolled = "binary";
				else if (roll < 0.33f) //PROTOBUF
					methodRolled = "protobuf";
				else if (roll < 0.5f) //JSON
					methodRolled = "JSON";
				else if (roll < 0.66f) //XML
					methodRolled = "XML";
				else if (roll < 0.83f) //YAML
					methodRolled = "YAML";
				else //CSV
					methodRolled = "CSV";

				logger?.Log<PersistentTimerWithSerializationSampleBehaviour>(
					$"ACCUMULATING PERSISTENT TIMER DESERIALIZED. METHOD: \"{methodRolled}\" PROGRESS: HOURS: {timeProgress.Hours.ToString()} MINUTES: {timeProgress.Minutes.ToString()} SECONDS: {timeProgress.Seconds.ToString()}");
			}

			return result;
		}
	}
}