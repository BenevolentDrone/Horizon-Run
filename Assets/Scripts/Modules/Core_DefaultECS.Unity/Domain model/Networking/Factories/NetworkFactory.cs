using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Messaging;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.ECS;
using HereticalSolutions.Networking.LiteNetLib;

using HereticalSolutions.Templates.Universal.Networking;

using HereticalSolutions.Logging;

using LiteNetLib;
using LiteNetLib.Utils;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking.Factories
{
    public static class NetworkFactory
    {
        private const int INITIAL_SERIALIZER_POOL_CAPACITY = 5;
        
        private const int ADDITIONAL_SERIALIZER_POOL_CAPACITY = 5;
        
        public static NetworkEntityManager BuildNetworkEntityManager(
            NetworkTickSettings networkTickSettings,
            NetworkRollCallSettings networkRollCallSettings,
            NetworkDespawnFailsafeSettings networkDespawnFailsafeSettings,

            NetworkPlayerSettings playerSettings,

            INetworkHost networkHost,
            INetworkClient networkClient,
            
            UniversalTemplateEntityManager entityManager,
            IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
            INonAllocSubscribableNoArgs pinger,
            
            INonAllocMessageSender networkBusAsSender,
            INonAllocMessageReceiver networkBusAsReceiver,
            
            IComponentSerializerManager eventComponentSerializerManager,
            IComponentSerializerManager entityComponentSerializerManager,
            
            string relevantWorldID,
            World relevantWorld,
            
            ILoggerResolver loggerResolver = null)
        {
            var result = new NetworkEntityManager(
                networkTickSettings,
                networkRollCallSettings,
                networkDespawnFailsafeSettings,

                playerSettings,

                networkHost,
                networkClient,
                
                RepositoriesFactory.BuildDictionaryOneToOneMap<ushort, Guid>(),
                RepositoriesFactory.BuildDictionaryOneToOneMap<ushort, string>(),
                
                RepositoriesFactory.BuildDictionaryRepository<ushort, DateTime>(),
                
                entityManager,
                eventEntityBuilder,
                BuildDeltaPacketProcessors(),
                pinger,
                
                networkBusAsSender,
                networkBusAsReceiver,
                
                eventComponentSerializerManager,
                entityComponentSerializerManager,
                
                new List<ushort>(),
                new List<EventPacket>(),
                
                relevantWorldID,
                relevantWorld,
                
                loggerResolver?.GetLogger<NetworkEntityManager>());

            
            
            return result;
        }
        
        public static ComponentSerializerManager BuildComponentSerializerManager(
            IPool<IComponentSerializer>[] componentSerializersPool,
            ESerializedEntityType serializedEntityType,
            ComponentSerializationContext componentSerializationContext,
            ILoggerResolver loggerResolver = null)
        {
            return new ComponentSerializerManager(
                componentSerializersPool,
                serializedEntityType,
                componentSerializationContext,
                new List<ushort>(),
                new List<IComponentSerializer>(),
                loggerResolver?.GetLogger<ComponentSerializerManager>());
        }

        public static IPool<IComponentSerializer>[] BuildEventComponentSerializersPool(
            ILoggerResolver loggerResolver = null)
        {
            return new IPool<IComponentSerializer>[]
            {
                //General purpose event components
                BuildComponentSerializerPool<EventSourceEntitySerializer>(loggerResolver),
                BuildComponentSerializerPool<EventSourceEntitySubaddressSerializer>(loggerResolver),
                BuildComponentSerializerPool<EventReceiverEntitySerializer>(loggerResolver),
                BuildComponentSerializerPool<EventReceiverEntitySubaddressSerializer>(loggerResolver),
                BuildComponentSerializerPool<EventTargetEntitySerializer>(loggerResolver),
                BuildComponentSerializerPool<EventTargetEntitySubaddressSerializer>(loggerResolver),
                BuildComponentSerializerPool<EventPositionSerializer>(loggerResolver),
                BuildComponentSerializerPool<EventTargetPositionSerializer>(loggerResolver),
                BuildComponentSerializerPool<EventTimeSerializer>(loggerResolver)
            };
        }
        
        public static IPool<IComponentSerializer>[] BuildEntityComponentSerializersPool(
            ILoggerResolver loggerResolver = null)
        {
            return new IPool<IComponentSerializer>[]
            {
                //Components
                
                BuildComponentSerializerPool<Position2DSerializer>(loggerResolver),
                BuildComponentSerializerPool<Position3DSerializer>(loggerResolver),
                BuildComponentSerializerPool<UniformRotationSerializer>(loggerResolver),
                BuildComponentSerializerPool<QuaternionSerializer>(loggerResolver)
            };
        }
        
        private static IPool<IComponentSerializer> BuildComponentSerializerPool<TSerializer>(
            ILoggerResolver loggerResolver = null)
        {
            Func<IComponentSerializer> valueAllocationDelegate =
                AllocationsFactory.ActivatorAllocationDelegate<IComponentSerializer, TSerializer>;

            var initialAllocationCommand = new AllocationCommand<IComponentSerializer>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = INITIAL_SERIALIZER_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            var additionalAllocationCommand = new AllocationCommand<IComponentSerializer>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
                    
                    Amount = ADDITIONAL_SERIALIZER_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };

            return StackPoolFactory.BuildStackPool(
                initialAllocationCommand,
                additionalAllocationCommand,
                loggerResolver);
        }

        public static NetworkHost BuildNetworkHost(
            NetworkBasicSettings basicSettings,
            NetworkDefaultConnectionValuesSettings defaultConnectionValuesSettings,
            NetworkTickSettings tickSettings,
            NetworkTimeoutSettings timeoutSettings,
            NetworkLagSimulationSettings lagSimulationSettings,

            INonAllocSubscribableNoArgs pinger,
            
            INonAllocMessageSender networkBusAsSender,
            INonAllocMessageReceiver networkBusAsReceiver,
            
            IPacketRepository packetRepository,
            
            ILoggerResolver loggerResolver = null)
        {
            var packetProcessor = new NetPacketProcessor();
            
            InitializeNetPacketProcessor(packetProcessor);

            var connections = new ServerToClientConnectionDescriptor[basicSettings.MaxConnections];
            
            for (int i = 0; i < connections.Length; i++)
            {
                connections[i].PeerID = -1;
            }
            
            var result = new NetworkHost(
                basicSettings,
                defaultConnectionValuesSettings,
                tickSettings,

                pinger,
                
                networkBusAsSender,
                networkBusAsReceiver,
                
                packetRepository,
                
                new NetDataWriter(),
                packetProcessor,
                
                connections,
                loggerResolver?.GetLogger<NetworkHost>());
            
            var netManager = new NetManager(
                result)
            {
                AutoRecycle = true,
                EnableStatistics = true
            };

            netManager.SimulateLatency = lagSimulationSettings.SimulateLag;
            
            netManager.DisconnectTimeout = timeoutSettings.DisconnectTimeoutInMs;
            
            netManager.SimulatePacketLoss = lagSimulationSettings.SimulatePacketLoss;

            result.NetManager = netManager;
            
            return result;
        }

        public static NetworkClient BuildNetworkClient(
            NetworkPlayerSettings playerSettings,

            NetworkBasicSettings networkBasicSettings,
            NetworkLagSimulationSettings lagSimulationSettings,

            INonAllocSubscribableNoArgs pinger,
            
            INonAllocMessageSender networkBusAsSender,
            INonAllocMessageReceiver networkBusAsReceiver,
            INonAllocMessageSender gameStateBusAsSender,
            
            IPacketRepository packetRepository,
            
            ILoggerResolver loggerResolver = null)
        {
            var packetProcessor = new NetPacketProcessor();
            
            InitializeNetPacketProcessor(packetProcessor);
            
            var result = new NetworkClient(
                playerSettings,
                
                pinger,
                
                networkBusAsSender,
                networkBusAsReceiver,
                gameStateBusAsSender,
                
                packetRepository,
                
                new NetDataWriter(),
                packetProcessor,
                
                loggerResolver?.GetLogger<NetworkHost>());
            
            var netManager = new NetManager(
                result)
            {
                AutoRecycle = true,
                IPv6Enabled = false,
                EnableStatistics = true
            };
            
            netManager.SimulateLatency = lagSimulationSettings.SimulateLag;
            
            netManager.SimulatePacketLoss = lagSimulationSettings.SimulatePacketLoss;

            result.NetManager = netManager;
            
            return result;
        }

        public static void InitializeNetPacketProcessor(
            NetPacketProcessor packetProcessor)
        {
            #region Vector2
            
            packetProcessor.RegisterNestedType(
                (writer, vector2) => writer.Put(vector2), 
                reader => reader.GetVector2());
            
            #endregion
            
            #region Vector3
            
            packetProcessor.RegisterNestedType(
                (writer, vector3) => writer.Put(vector3), 
                reader => reader.GetVector3());
            
            #endregion
            
            #region Quaternion
            
            packetProcessor.RegisterNestedType(
                (writer, quaternion) => writer.Put(quaternion), 
                reader => reader.GetQuaternion());
            
            #endregion
            
            
            packetProcessor.RegisterNestedType<IDPosition2DPair>();
            
            packetProcessor.RegisterNestedType<IDUniformRotationPair>();
            
            packetProcessor.RegisterNestedType<IDQuaternionPair>();
        }

        public static PacketRepository BuildPacketRepository()
        {
            TypeHelpers.GetTypesWithAttribute<PacketAttribute>(
                out var packetTypes);
            
            Array.Sort(
                packetTypes,
                (a, b) =>
                {
                    return string.CompareOrdinal(
                        a.Name,
                        b.Name);
                });
            
            IReadOnlyRepository<byte, Type> packetIDToType = RepositoriesFactory.BuildDictionaryRepository<byte, Type>();

            IReadOnlyRepository<Type, byte> typeToPacketID = RepositoriesFactory.BuildDictionaryRepository<Type, byte>();
            
            for (byte i = 0; i < packetTypes.Length; i++)
            {
                ((IRepository<byte, Type>)packetIDToType).AddOrUpdate(i, packetTypes[i]);
                
                ((IRepository<Type, byte>)typeToPacketID).AddOrUpdate(packetTypes[i], i);
            }

            return new PacketRepository(
                packetIDToType,
                typeToPacketID);
        }

        public static IDeltaPacketProcessor[] BuildDeltaPacketProcessors()
        {
            return new IDeltaPacketProcessor[]
            {
                new Position2DDeltaProcessor(),
                new Position3DDeltaProcessor(),
                new UniformRotationDeltaProcessor(),
                new QuaternionDeltaProcessor()
            };
        }
    }
}