using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

using HereticalSolutions.Repositories;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.ECS;
using HereticalSolutions.Networking.LiteNetLib;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using LiteNetLib;
using LiteNetLib.Utils;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    /*
    public class NetworkEntityManager
        : INetworkEntityManager<Guid, Entity>
    {
        private readonly NetworkTickSettings networkTickSettings;

        private readonly NetworkRollCallSettings networkRollCallSettings;

        private readonly NetworkDespawnFailsafeSettings networkDespawnFailsafeSettings;


        private readonly NetworkPlayerSettings playerSettings;

        
        private readonly INetworkHost networkHost;
        
        private readonly INetworkClient networkClient;
        
        
        private readonly IOneToOneMap<ushort, Guid> networkIDToEntityIDMap;

        private readonly IOneToOneMap<ushort, string> ushortIDToPrototypeIDMap;


        private readonly IRepository<ushort, DateTime> lastTimeEntityDataRequested;
        
        
        private readonly EntityManager entityManager;

        private readonly IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

        private readonly IDeltaPacketProcessor[] deltaPacketProcessors;

        private readonly INonAllocSubscribable pinger;

        #region Message bus

        private readonly INonAllocMessageSender networkBusAsSender;

        private readonly INonAllocMessageReceiver networkBusAsReceiver;

        #endregion

        #region Subscriptions

        private INonAllocSubscription pingSubscription;


        private readonly INonAllocSubscription clientJoinedServerSubscription;

        private readonly INonAllocSubscription clientReceivedPacketSubscription;

        private readonly INonAllocSubscription clientDisconnectedSubscription;


        private readonly INonAllocSubscription serverStartedSubscription;

        private readonly INonAllocSubscription serverReceivedPacketSubscription;

        private readonly INonAllocSubscription serverStoppedSubscription;

        #endregion

        #region Component serialization

        private readonly IComponentSerializerManager eventComponentSerializerManager;
        
        private readonly IComponentSerializerManager entityComponentSerializerManager;
        
        
        private readonly Action<INetSerializable, NetDataWriter> eventPacketSerializationAction;
        
        private readonly Action<INetSerializable, NetDataWriter> entityPacketSerializationAction;
        
        #endregion

        
        private readonly List<ushort> newNetworkEntities;
        
        private readonly List<EventPacket> eventPacketCache;
        
        
        private readonly string relevantWorldID;

        private readonly World relevantWorld;

        private readonly ILogger logger;


        private ushort lastAllocatedNetworkID;

        private ushort vacantUShortPrototypeID;

        private ushort rollCallCountdown;

        private bool anyNetworkEntityReceived = false;

        
        private bool isServer;

        private bool isClient;

        public NetworkEntityManager(
            NetworkTickSettings networkTickSettings,
            NetworkRollCallSettings networkRollCallSettings,
            NetworkDespawnFailsafeSettings networkDespawnFailsafeSettings,

            NetworkPlayerSettings playerSettings,
            
            INetworkHost networkHost,
            INetworkClient networkClient,
            
            IOneToOneMap<ushort, Guid> networkIDToEntityIDMap,
            IOneToOneMap<ushort, string> ushortIDToPrototypeIDMap,
            
            IRepository<ushort, DateTime> lastTimeEntityDataRequested,
            
            EntityManager entityManager,
            IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
            IDeltaPacketProcessor[] deltaPacketProcessors,
            INonAllocSubscribable pinger,
            
            INonAllocMessageSender networkBusAsSender,
            INonAllocMessageReceiver networkBusAsReceiver,
            
            IComponentSerializerManager eventComponentSerializerManager,
            IComponentSerializerManager entityComponentSerializerManager,
            
            List<ushort> newNetworkEntities,
            List<EventPacket> eventPacketCache,
            
            string relevantWorldID,
            World relevantWorld,
            ILogger logger)
        {
            this.networkTickSettings = networkTickSettings;

            this.networkRollCallSettings = networkRollCallSettings;

            this.networkDespawnFailsafeSettings = networkDespawnFailsafeSettings;


            this.playerSettings = playerSettings;
            

            this.networkHost = networkHost;
            
            this.networkClient = networkClient;
            
            
            this.networkIDToEntityIDMap = networkIDToEntityIDMap;

            this.ushortIDToPrototypeIDMap = ushortIDToPrototypeIDMap;
            
            
            this.lastTimeEntityDataRequested = lastTimeEntityDataRequested;

            
            this.entityManager = entityManager;
            
            this.eventEntityBuilder = eventEntityBuilder;

            this.deltaPacketProcessors = deltaPacketProcessors;

            this.pinger = pinger;

            
            this.networkBusAsSender = networkBusAsSender;

            this.networkBusAsReceiver = networkBusAsReceiver;


            this.eventComponentSerializerManager = eventComponentSerializerManager;
            
            this.entityComponentSerializerManager = entityComponentSerializerManager;


            eventPacketSerializationAction = (packet, writer) =>
            {
                if (packet is IContainsComponentData packetWithComponentData)
                {
                    eventComponentSerializerManager.Serialize(
                        packetWithComponentData,
                        writer);
                }
                else
                {
                    packet.Serialize(writer);
                }
            };
            
            entityPacketSerializationAction = (packet, writer) =>
            {
                if (packet is IContainsComponentData packetWithComponentData)
                {
                    entityComponentSerializerManager.Serialize(
                        packetWithComponentData,
                        writer);
                }
                else
                {
                    packet.Serialize(writer);
                }
            };
            
            
            this.newNetworkEntities = newNetworkEntities;
            
            this.eventPacketCache = eventPacketCache;
            
            
            this.relevantWorldID = relevantWorldID;

            this.relevantWorld = relevantWorld;

            this.logger = logger;


            lastAllocatedNetworkID = 0;

            vacantUShortPrototypeID = 0;

            rollCallCountdown = this.networkRollCallSettings.RollCallFrequency;
            
            anyNetworkEntityReceived = false;


            isServer = false;

            isClient = false;


            pingSubscription = DelegatesFactory.BuildSubscriptionNoArgs(
                OnPing);


            clientJoinedServerSubscription =
                DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientJoinedServerMessage>(
                    OnClientJoinedServer);

            clientReceivedPacketSubscription =
                DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientReceivedPacketMessage>(
                    OnClientReceivedPacketMessage);

            clientDisconnectedSubscription =
                DelegatesFactory.BuildSubscriptionSingleArgGeneric<ClientDisconnectedMessage>(
                    OnClientDisconnected);


            serverStartedSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ServerStartedMessage>(
                OnServerStarted);

            serverReceivedPacketSubscription =
                DelegatesFactory.BuildSubscriptionSingleArgGeneric<ServerReceivedPacketMessage>(
                    OnServerReceivedPacketMessage);

            serverStoppedSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ServerStoppedMessage>(
                OnServerStopped);

            //Due to the fact that we have no proper scene lifetime YET, I will leave these subscriptions untouched for a while
            //For you see, before the scene manages to unload the manager manages to catch the message and subscribe
            //Staying in the RAM after the scene is destroyed as a dangling subswcriber still reading the fucking messages
            
            //networkBusAsReceiver.SubscribeTo<ClientJoinedServerMessage>(
            //    clientJoinedServerSubscription);

            //networkBusAsReceiver.SubscribeTo<ClientDisconnectedMessage>(
            //    clientDisconnectedSubscription);


            //networkBusAsReceiver.SubscribeTo<ServerStartedMessage>(
            //    serverStartedSubscription);

            //networkBusAsReceiver.SubscribeTo<ServerStoppedMessage>(
            //    serverStoppedSubscription);
        }


        #region INetworkEntityManager

        #region INetworkEntityRepository

        public bool TryAllocateNetworkEntityID(
            out ushort networkID)
        {
            var cachedLastAllocatedNetworkID = lastAllocatedNetworkID;

            do
            {
                if (lastAllocatedNetworkID == ushort.MaxValue)
                    lastAllocatedNetworkID = 0;
                else
                    lastAllocatedNetworkID++;

                if (lastAllocatedNetworkID == cachedLastAllocatedNetworkID)
                    throw new Exception(
                        logger.TryFormatException<NetworkEntityManager>(
                            $"NETWORK ID POOL EXHAUSTED"));
            } while (networkIDToEntityIDMap.HasLeft(lastAllocatedNetworkID));

            networkID = lastAllocatedNetworkID;

            return true;
        }

        public bool HasNetworkEntity(
            ushort networkID)
        {
            return networkIDToEntityIDMap.HasLeft(networkID);
        }

        public bool TryAddNetworkEntity(
            ushort networkID,
            Guid entityID)
        {
            var result = networkIDToEntityIDMap.TryAdd(
                networkID,
                entityID);

            if (result)
            {
                newNetworkEntities.Add(networkID);
            }

            return result;
        }

        public bool TryRemoveNetworkEntity(
            ushort networkID)
        {
            return networkIDToEntityIDMap.TryRemoveByLeft(networkID);
        }

        public bool TryGetEntityID(
            ushort networkID,
            out Guid entityID)
        {
            return networkIDToEntityIDMap.TryGetRight(
                networkID,
                out entityID);
        }

        public bool TryGetNetworkID(
            Guid entityID,
            out ushort networkID)
        {
            return networkIDToEntityIDMap.TryGetLeft(
                entityID,
                out networkID);
        }

        #endregion

        #region INetworkEntityPrototypeRepository

        public void RegisterPrototype(string prototypeID)
        {
            if (ushortIDToPrototypeIDMap.HasRight(prototypeID))
                return;

            ushortIDToPrototypeIDMap.AddOrUpdateByLeft(
                vacantUShortPrototypeID,
                prototypeID);

            vacantUShortPrototypeID++;
        }

        public bool TryGetUShortPrototypeID(
            string prototypeID,
            out ushort ushortPrototypeID)
        {
            return ushortIDToPrototypeIDMap.TryGetLeft(
                prototypeID,
                out ushortPrototypeID);
        }

        public bool TryGetStringPrototypeID(
            ushort prototypeID,
            out string stringPrototypeID)
        {
            return ushortIDToPrototypeIDMap.TryGetRight(
                prototypeID,
                out stringPrototypeID);
        }

        #endregion

        #region INetworkEventEntityManager

        public void ReplicateEventEntity(Entity entity)
        {
            if (ParseEventEntity(
                entity,
                out var eventPacket))
                eventPacketCache.Add(eventPacket);
        }

        #endregion
        
        public void StartInClientMode()
        {
            if (isClient || isServer)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"ABORTING StartInClientMode: isClient {isClient} isServer {isServer}");
                
                return;
            }
            
            //Once the client is started and connected, we know we're the receiver
            isClient = true;

            isServer = false;

            //Start listening to packets
            if (!clientReceivedPacketSubscription.Active)
            {
                networkBusAsReceiver.SubscribeTo<ClientReceivedPacketMessage>(
                    clientReceivedPacketSubscription);
            }
            
            if (pingSubscription.Active)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"PING SUBSCRIPTION IS STILL ACTIVE");
                
                pingSubscription.Unsubscribe();
            }
            
            pinger.Subscribe(
                pingSubscription);

            //Send a request for the world info to know the server entities
            networkBusAsSender
                .PopMessage<ClientSendPacketMessage>(
                    out var sendPacketMessage)
                .Write<ClientSendPacketMessage>(
                    sendPacketMessage,
                    new object[]
                    {
                        typeof(RequestWorldInfoPacket),
                        new RequestWorldInfoPacket(),
                        null,
                        DeliveryMethod.ReliableUnordered
                    })
                .SendImmediately<ClientSendPacketMessage>(
                    sendPacketMessage);
        }

        public void StartInServerMode()
        {
            if (isClient || isServer)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"ABORTING StartInServerMode: isClient {isClient} isServer {isServer}");
                
                return;
            }
            
            //Once the server is started, we know we're the sender
            isServer = true;

            isClient = false;

            //Start listening to packets
            if (!serverReceivedPacketSubscription.Active)
            {
                networkBusAsReceiver.SubscribeTo<ServerReceivedPacketMessage>(
                    serverReceivedPacketSubscription);
            }
            
            if (pingSubscription.Active)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"PING SUBSCRIPTION IS STILL ACTIVE");
                
                pingSubscription.Unsubscribe();
            }
            
            pinger.Subscribe(
                pingSubscription);
        }

        public void Stop()
        {
            //Once the client is disconnected, we know we're not the receiver anymore
            isClient = false;

            isServer = false;
            
            //Stop listening to packets
            if (clientReceivedPacketSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ClientReceivedPacketMessage>(
                    clientReceivedPacketSubscription);
            }
            
            if (serverReceivedPacketSubscription.Active)
            {
                logger?.Log<NetworkEntityManager>(
                    $"UNSUBSCRIBING serverReceivedPacketSubscription");
                
                networkBusAsReceiver.UnsubscribeFrom<ServerReceivedPacketMessage>(
                    serverReceivedPacketSubscription);
            }
            
            if (pingSubscription.Active)
            {
                pinger.Unsubscribe(
                    pingSubscription);
            }
        }

        #endregion

        #region Message handlers

        private void OnClientJoinedServer(ClientJoinedServerMessage message)
        {
            StartInClientMode();
        }
        
        private void OnClientReceivedPacketMessage(ClientReceivedPacketMessage message)
        {
            if (networkClient.Status != EClientStatus.CONNECTED)
            {
                logger?.Log<NetworkEntityManager>(
                    $"PACKET FROM SERVER DROPPED BECAUSE THE PLAYER IS NOT CONNECTED YET");
                
                return;
            }

            if (ProcessNetworkEntityRollCallPacket(message))
                return;
            
            if (ProcessEntityCreatedPacket(message))
                return;
            
            if (ProcessEntityDestroyedPacket(message))
                return;

            foreach (var deltaPacketProcessor in deltaPacketProcessors)
                if (deltaPacketProcessor.ProcessDeltasPacket(
                    message,
                    networkTickSettings,
                    entityManager,
                    networkIDToEntityIDMap,
                    relevantWorldID))
                    return;
            
            if (ClientProcessEventBatchPacket(message))
                return;
            
            if (ClientProcessEventPacket(message))
                return;
        }
        
        private void OnClientDisconnected(ClientDisconnectedMessage message)
        {
            Stop();
        }
        
        private void OnServerStarted(ServerStartedMessage message)
        {
            StartInServerMode();
        }
        
        private void OnServerReceivedPacketMessage(ServerReceivedPacketMessage message)
        {
            var playerSlot = message.PlayerSlot;

            if (networkHost.Connections[playerSlot].Status != EServerToClientConnectionStatus.CONNECTED)
            {
                logger?.Log<NetworkEntityManager>(
                    $"PACKET FROM SLOT {playerSlot} DROPPED BECAUSE THE PLAYER IS NOT CONNECTED YET");
                
                return;
            }

            if (ProcessRequestWorldInfoPacket(message))
                return;
            
            if (ProcessRequestEntityInfoPacket(message))
                return;
            
            if (ServerProcessEventBatchPacket(message))
                return;
            
            if (ServerProcessEventPacket(message))
                return;
        }
        
        private void OnServerStopped(ServerStoppedMessage message)
        {
            Stop();
        }
        
        #endregion

        #region Packet handlers

        private bool ProcessNetworkEntityRollCallPacket(ClientReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(NetworkEntityRollCallPacket))
                return false;

            var packet = new NetworkEntityRollCallPacket();
            
            packet.Deserialize(message.Reader);
            
            //Kill all entities that are not mentioned in the roll call
            var lastNetworkIDs = networkIDToEntityIDMap.LeftValues.ToArray();
            
            KillNetworkEntitiesNotMentionedByRollCall(
                lastNetworkIDs,
                packet);

            int currentEntityInfoRequests = 0;
            
            //Ask for the info on entities that the manager is unaware of
            RequestInfoOnUnknownNetworkEntities(
                packet,
                ref currentEntityInfoRequests);

            return true;
        }

        private void KillNetworkEntitiesNotMentionedByRollCall(
            ushort[] lastNetworkIDs,
            NetworkEntityRollCallPacket packet)
        {
            if (lastNetworkIDs == null
                || lastNetworkIDs.Length == 0
                || packet.NetworkIDs == null
                || packet.NetworkIDs.Length == 0)
                return;

            var missingNetworkIDs = lastNetworkIDs.Except(packet.NetworkIDs);
            
            //foreach (var networkID in lastNetworkIDs)
            foreach (var networkID in missingNetworkIDs)
            {
                //if (!packet.NetworkIDs.Contains(networkID))
                //{
                    var entityID = networkIDToEntityIDMap.GetRight(networkID);

                    var registryEntity = entityManager.GetRegistryEntity(entityID);

                    if (!registryEntity.Has<NetworkEntityComponent>())
                    {
                        string currentPrototypeID = "";
                        
                        if (registryEntity.IsAlive
                            && registryEntity.Has<PrototypeInstanceComponent>())
                            currentPrototypeID = registryEntity.Get<PrototypeInstanceComponent>().PrototypeID;
                        
                        logger?.LogError<NetworkEntityManager>(
                            $"ENTITY {entityID} FROM ROLL CALL LIST HAS NO NetworkEntityComponent, DESPAWNING. PROTOTYPE ID: {currentPrototypeID}");
                        
                        entityManager.DespawnEntity(entityID);
                    
                        networkIDToEntityIDMap.TryRemoveByLeft(networkID);

                        continue;
                    }

                    var networkEntityComponent = registryEntity.Get<NetworkEntityComponent>();

                    var lastTick = networkEntityComponent.LastReceivedServerTick;
                    
                    var currentTick = packet.ServerTick;

                    if (currentTick < lastTick
                        && ((lastTick - currentTick) > networkTickSettings.MaxTickValue / 2f))
                        currentTick += networkTickSettings.MaxTickValue;

                    if (currentTick - lastTick > networkDespawnFailsafeSettings.DespawnFailsafeTicks)
                    {
                        logger?.LogError<NetworkEntityManager>(
                            $"ENTITY {entityID} IS NOT ON A ROLL CALL LIST AND HAS EXCEEDED FAILSAFE TICKS, DESPAWNING");
                        
                        entityManager.DespawnEntity(entityID);

                        networkIDToEntityIDMap.TryRemoveByLeft(networkID);
                    }
                //}
            }
        }

        private void RequestInfoOnUnknownNetworkEntities(
            NetworkEntityRollCallPacket packet,
            ref int currentEntityInfoRequests)
        {
            foreach (var networkID in packet.NetworkIDs)
            {
                if (!networkIDToEntityIDMap.HasLeft(networkID))
                {
                    //logger?.Log<NetworkEntityManager>(
                    //    $"NETWORK ENTITY NOT FOUND: {networkID}");
                    
                    if (!anyNetworkEntityReceived)
                    {
                        //logger?.Log<NetworkEntityManager>(
                        //    $"DISCONTINUING BECAUSE WE HAVE NOT RECEIVED ANY NETWORK ENTITY DATA");
                        
                        continue;
                    }

                    if (currentEntityInfoRequests >= networkRollCallSettings.MaxEntityInfoRequestsPerRollCall)
                    {
                        //logger?.Log<NetworkEntityManager>(
                        //    $"DISCONTINUING BECAUSE WE HAVE ALREADY SENT MORE REQUESTS THAN {networkSettings.MaxEntityInfoRequestsPerRollCall}");
                        
                        continue;
                    }

                    bool haveBeenRequestedTooOften =
                        lastTimeEntityDataRequested.Has(networkID)
                        && (DateTime.Now - lastTimeEntityDataRequested.Get(networkID)).TotalSeconds < networkRollCallSettings.DelayBetweenRepeatedEntityInfoRequests;

                    if (haveBeenRequestedTooOften)
                    {
                        //logger?.Log<NetworkEntityManager>(
                        //    $"DISCONTINUING BECAUSE WE HAVE ALREADY REQUESTED INFO ON THIS ENTITY RECENTLY");
                        
                        continue;
                    }

                    logger?.Log<NetworkEntityManager>(
                        $"ENTITY {networkID} FROM ROLL CALL LIST IS UNKNOWN, ASKING FOR INFO");
                    
                    networkBusAsSender
                        .PopMessage<ClientSendPacketMessage>(
                            out var sendPacketMessage)
                        .Write<ClientSendPacketMessage>(
                            sendPacketMessage,
                            new object[]
                            {
                                typeof(RequestEntityInfoPacket),
                                new RequestEntityInfoPacket
                                {
                                    NetworkID = networkID
                                },
                                null,
                                DeliveryMethod.ReliableUnordered
                            })
                        .SendImmediately<ClientSendPacketMessage>(
                            sendPacketMessage);
                    
                    lastTimeEntityDataRequested.AddOrUpdate(
                        networkID,
                        DateTime.Now);

                    currentEntityInfoRequests++;
                    
                    continue;
                }
                
                var entityID = networkIDToEntityIDMap.GetRight(networkID);

                var registryEntity = entityManager.GetRegistryEntity(entityID);

                if (!registryEntity.Has<NetworkEntityComponent>())
                {
                    string currentPrototypeID = "";
                        
                    if (registryEntity.IsAlive
                        && registryEntity.Has<PrototypeInstanceComponent>())
                        currentPrototypeID = registryEntity.Get<PrototypeInstanceComponent>().PrototypeID;
                    
                    logger?.LogError<NetworkEntityManager>(
                        $"ENTITY {entityID} FROM ROLL CALL LIST HAS NO NetworkEntityComponent. PROTOTYPE ID: {currentPrototypeID}");
                    
                    continue;
                }

                ref var networkEntityComponent = ref registryEntity.Get<NetworkEntityComponent>();
                
                networkEntityComponent.LastReceivedServerTick = packet.ServerTick;
            }
        }

        private bool ProcessEntityCreatedPacket(ClientReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(EntityCreatedPacket))
                return false;

            var packet = new EntityCreatedPacket();

            entityComponentSerializerManager.Deserialize(
                packet,
                message.Reader);

            anyNetworkEntityReceived = true;

            //If we already know about entity then update its data if packet's data is newer than the one we have
            if (networkIDToEntityIDMap.HasLeft(packet.NetworkID))
            {
                TryUpdateKnownNetworkEntityData(packet);

                return true;
            }

            var entityID = new Guid(packet.GUID);
            
            //If the entity by the given ID does not exist, let's create one
            if (!entityManager.HasEntity(entityID))
            {
                CreateNewNetworkEntity(
                    packet,
                    entityID);
            }
            else
            {
                PopulateNetworkEntity(
                    packet,
                    entityID);
            }

            //Memorize entity as networked entity
            networkIDToEntityIDMap.TryAdd(
                packet.NetworkID,
                entityID);
            
            return true;
        }

        private void TryUpdateKnownNetworkEntityData(
            EntityCreatedPacket packet)
        {
            var knownNetworkEntityID = networkIDToEntityIDMap.GetRight(packet.NetworkID);

            var registryEntity = entityManager.GetRegistryEntity(knownNetworkEntityID);

            if (!registryEntity.Has<NetworkEntityComponent>())
            {
                string currentPrototypeID = "";
                        
                if (registryEntity.IsAlive
                    && registryEntity.Has<PrototypeInstanceComponent>())
                    currentPrototypeID = registryEntity.Get<PrototypeInstanceComponent>().PrototypeID;
                
                logger?.LogError<NetworkEntityManager>(
                    $"ENTITY {knownNetworkEntityID} FROM ENTITY CREATED PACKET HAS NO NetworkEntityComponent. PROTOTYPE ID: {currentPrototypeID}");
                    
                return;
            }

            ref var networkEntityComponent = ref registryEntity.Get<NetworkEntityComponent>();

            if (packet.ServerTick > networkEntityComponent.LastReceivedServerTick
                && (packet.ServerTick - networkEntityComponent.LastReceivedServerTick) <
                networkTickSettings.MaxTickValue / 2)
            {
                var knownNetworkSimulationEntity = entityManager.GetEntity(
                    knownNetworkEntityID,
                    relevantWorldID);
            
                entityComponentSerializerManager.PopulateEntity(
                    knownNetworkSimulationEntity,
                    packet.Serializers,
                    packet.SerializerIDs);
                
                logger?.Log<NetworkEntityManager>(
                    $"NETWORK ENTITY {packet.NetworkID} WITH GUID {knownNetworkEntityID} ALREADY CREATED, POPULATING VALUES. PROTOTYPE: {entityManager.GetRegistryEntity(knownNetworkEntityID).Get<PrototypeInstanceComponent>().PrototypeID}");
            }
            else
            {
                //logger?.Log<NetworkEntityManager>(
                //    $"INFORMATION IS OUTDATED, IGNORING PACKET");
            }
        }

        private void CreateNewNetworkEntity(
            EntityCreatedPacket packet,
            Guid entityID)
        {
            if (!ushortIDToPrototypeIDMap.HasLeft(
                packet.PrototypeID))
                logger?.LogError<NetworkEntityManager>(
                    $"NO PROTOTYPE ID BY INDEX {packet.PrototypeID} FOUND");

            string prototypeID = ushortIDToPrototypeIDMap.GetRight(
                packet.PrototypeID);

            Entity @override = relevantWorld.CreateEntity();
            
            entityComponentSerializerManager.PopulateEntity(
                @override,
                packet.Serializers,
                packet.SerializerIDs);
            
            entityManager.SpawnEntity(
                entityID,
                prototypeID,
                new WorldOverrideDescriptor<Entity>[]
                {
                    new WorldOverrideDescriptor<Entity>()
                    {
                        WorldID = relevantWorldID,
                        
                        OverrideEntity = @override
                    }
                },
                EEntityAuthoringPresets.NETWORKING_CLIENT);

            var registryEntity = entityManager.GetRegistryEntity(entityID);

            if (!registryEntity.Has<NetworkEntityComponent>())
            {
                string currentPrototypeID = "";
                        
                if (registryEntity.IsAlive
                    && registryEntity.Has<PrototypeInstanceComponent>())
                    currentPrototypeID = registryEntity.Get<PrototypeInstanceComponent>().PrototypeID;
                
                throw new Exception(
                    logger.TryFormatException<NetworkEntityManager>(
                        $"ENTITY {entityID} FROM ENTITY CREATED PACKET HAS NO NetworkEntityComponent. PROTOTYPE ID: {currentPrototypeID}"));
            }

            
            ref var networkEntityComponent = ref registryEntity.Get<NetworkEntityComponent>();
                
            networkEntityComponent.NetworkID = packet.NetworkID;

            networkEntityComponent.LastReceivedServerTick = packet.ServerTick;

            
            if (registryEntity.Has<AuthoringPermissionComponent>())
            {
                ref var authoringPermissionComponent = ref registryEntity.Get<AuthoringPermissionComponent>();

                authoringPermissionComponent.PlayerSlot = packet.AuthoringPermission;
            }

            logger?.Log<NetworkEntityManager>(
                $"NETWORK ENTITY {packet.NetworkID} SPAWNED, GUID: {entityID} PROTOTYPE: {prototypeID}");
        }

        private void PopulateNetworkEntity(
            EntityCreatedPacket packet,
            Guid entityID)
        {
            var simulationEntity = entityManager.GetEntity(
                entityID,
                relevantWorldID);
            
            entityComponentSerializerManager.PopulateEntity(
                simulationEntity,
                packet.Serializers,
                packet.SerializerIDs);
        }

        private bool ProcessEntityDestroyedPacket(ClientReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(EntityDestroyedPacket))
                return false;

            var packet = new EntityDestroyedPacket();

            packet.Deserialize(message.Reader);
            
            
            logger?.Log<NetworkEntityManager>(
                $"DESTROYING NETWORK ENTITY {packet.NetworkID}");
            
            //If there's no entity like that anymore, we don't need to do anything
            if (!networkIDToEntityIDMap.HasLeft(packet.NetworkID))
            {
                logger?.Log<NetworkEntityManager>(
                    $"NETWORK ENTITY ALREADY DESTROYED");
                
                return true;
            }

            var entityID = networkIDToEntityIDMap.GetRight(packet.NetworkID);
            
            //If there is an entity by the given ID, let's destroy it
            if (entityManager.HasEntity(entityID))
                entityManager.DespawnEntity(entityID);
            
            //And forget about it
            networkIDToEntityIDMap.TryRemoveByLeft(
                packet.NetworkID);
            
            return true;
        }
        
        private bool ClientProcessEventBatchPacket(ClientReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(EventBatchPacket))
                return false;

            var packet = new EventBatchPacket();

            eventComponentSerializerManager.Deserialize(
                packet,
                message.Reader);
            
            
            foreach (var eventPacket in packet.Events)
            {
                ParseEventPacket(
                    eventPacket,
                    ENetworkEventSource.SERVER);
            }
            
            return true;
        }

        private bool ClientProcessEventPacket(ClientReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(EventPacket))
                return false;

            var packet = new EventPacket();
            
            eventComponentSerializerManager.Deserialize(
                packet,
                message.Reader);
            
            
            ParseEventPacket(
                packet,
                ENetworkEventSource.SERVER);
            
            return true;
        }

        private bool ProcessRequestWorldInfoPacket(ServerReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(RequestWorldInfoPacket))
                return false;

            var packet = new RequestWorldInfoPacket();

            packet.Deserialize(message.Reader);

            
            var networkIDs = networkIDToEntityIDMap.LeftValues.ToArray();

            foreach (var networkID in networkIDs)
            {
                if (!networkIDToEntityIDMap.TryGetRight(
                        networkID,
                        out var entityID))
                {
                    logger?.LogError<NetworkEntityManager>(
                        $"ENTITY ID NOT FOUND BY NETWORK ID {networkID}");
                    
                    continue;
                }
                
                var entity = entityManager.GetRegistryEntity(
                    entityID);

                if (!entity.Has<NetworkEntityComponent>())
                {
                    string currentPrototypeID = "";
                        
                    if (entity.IsAlive
                        && entity.Has<PrototypeInstanceComponent>())
                        currentPrototypeID = entity.Get<PrototypeInstanceComponent>().PrototypeID;
                    
                    throw new Exception(
                        logger.TryFormatException<NetworkEntityManager>(
                            $"ENTITY {entityID} HAS NO NetworkEntityComponent. PROTOTYPE ID: {currentPrototypeID}"));
                }

                if (!entity.Has<PrototypeInstanceComponent>())
                    throw new Exception(
                        logger.TryFormatException<NetworkEntityManager>(
                            $"ENTITY {entityID} HAS NO PrototypeInstanceComponent"));

                string prototypeID = entity.Get<PrototypeInstanceComponent>().PrototypeID;

                if (!ushortIDToPrototypeIDMap.TryGetLeft(
                        prototypeID,
                        out var prototypeIDAsUShort))
                {
                    throw new Exception(
                        logger.TryFormatException<NetworkEntityManager>(
                            $"PROTOTYPE ID {prototypeID} NOT REGISTERED IN THE MAP"));
                }
                
                byte authoringPermission = byte.MaxValue;

                if (entity.Has<AuthoringPermissionComponent>())
                {
                    authoringPermission = entity.Get<AuthoringPermissionComponent>().PlayerSlot;
                }

                var simulationEntity = entityManager.GetEntity(
                    entityID,
                    relevantWorldID);

                ushort count = entityComponentSerializerManager.ParseEntity(
                    simulationEntity,
                    out var serializerIDs,
                    out var serializers);
                
                networkBusAsSender
                    .PopMessage<ServerSendPacketMessage>(
                        out var sendPacketMessage)
                    .Write<ServerSendPacketMessage>(
                        sendPacketMessage,
                        new object[]
                        {
                            message.PlayerSlot,
                            typeof(EntityCreatedPacket),
                            new EntityCreatedPacket
                            {
                                ServerTick = networkHost.Tick,

                                NetworkID = networkID,
                                
                                AuthoringPermission = authoringPermission,

                                GUID = entityID.ToByteArray(),

                                PrototypeID = prototypeIDAsUShort,


                                ComponentDataCount = count,

                                SerializerIDs = serializerIDs,

                                Serializers = serializers
                            },
                            entityPacketSerializationAction,
                            DeliveryMethod.ReliableUnordered
                        })
                    .SendImmediately<ServerSendPacketMessage>(
                        sendPacketMessage);
            }

            return true;
        }
        
        private bool ProcessRequestEntityInfoPacket(ServerReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(RequestEntityInfoPacket))
                return false;

            var packet = new RequestEntityInfoPacket();

            packet.Deserialize(message.Reader);

            
            if (!networkIDToEntityIDMap.TryGetRight(
                packet.NetworkID,
                out var entityID))
                return true;
            
            var entity = entityManager.GetRegistryEntity(
                entityID);

            if (!entity.Has<NetworkEntityComponent>())
            {
                string currentPrototypeID = "";
                        
                if (entity.IsAlive
                    && entity.Has<PrototypeInstanceComponent>())
                    currentPrototypeID = entity.Get<PrototypeInstanceComponent>().PrototypeID;
                
                throw new Exception(
                    logger.TryFormatException<NetworkEntityManager>(
                        $"ENTITY {entityID} HAS NO NetworkEntityComponent. PROTOTYPE ID: {currentPrototypeID}"));
            }

            if (!entity.Has<PrototypeInstanceComponent>())
                throw new Exception(
                    logger.TryFormatException<NetworkEntityManager>(
                        $"ENTITY {entityID} HAS NO PrototypeInstanceComponent"));

            string prototypeID = entity.Get<PrototypeInstanceComponent>().PrototypeID;
            
            if (!ushortIDToPrototypeIDMap.TryGetLeft(
                prototypeID,
                out var prototypeIDAsUShort))
                throw new Exception(
                    logger.TryFormatException<NetworkEntityManager>(
                        $"PROTOTYPE ID {prototypeID} NOT REGISTERED IN THE MAP"));

            byte authoringPermission = byte.MaxValue;

            if (entity.Has<AuthoringPermissionComponent>())
            {
                authoringPermission = entity.Get<AuthoringPermissionComponent>().PlayerSlot;
            }
            
            var simulationEntity = entityManager.GetEntity(
                entityID,
                relevantWorldID);
            
            ushort count = entityComponentSerializerManager.ParseEntity(
                simulationEntity,
                out var serializerIDs,
                out var serializers);
            
            networkBusAsSender
                .PopMessage<ServerSendPacketMessage>(
                    out var sendPacketMessage)
                .Write<ServerSendPacketMessage>(
                    sendPacketMessage,
                    new object[]
                    {
                        message.PlayerSlot,
                        typeof(EntityCreatedPacket),
                        new EntityCreatedPacket
                        {
                            ServerTick = networkHost.Tick,
                            
                            NetworkID = packet.NetworkID,
                            
                            AuthoringPermission = authoringPermission,
                            
                            GUID = entityID.ToByteArray(),
                            
                            PrototypeID = prototypeIDAsUShort,
                            
                            
                            ComponentDataCount = count,
                            
                            SerializerIDs = serializerIDs,
                            
                            Serializers = serializers
                        },
                        entityPacketSerializationAction,
                        DeliveryMethod.ReliableUnordered
                    })
                .SendImmediately<ServerSendPacketMessage>(
                    sendPacketMessage);

            return true;
        }
        
        private bool ServerProcessEventBatchPacket(ServerReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(EventBatchPacket))
                return false;

            var packet = new EventBatchPacket();
            
            eventComponentSerializerManager.Deserialize(
                packet,
                message.Reader);
            
            
            foreach (var eventPacket in packet.Events)
            {
                ParseEventPacket(
                    eventPacket,
                    ENetworkEventSource.CLIENT);
            }
            
            return true;
        }

        private bool ServerProcessEventPacket(ServerReceivedPacketMessage message)
        {
            if (message.PacketType != typeof(EventPacket))
                return false;

            var packet = new EventPacket();
            
            eventComponentSerializerManager.Deserialize(
                packet,
                message.Reader);
            
            
            ParseEventPacket(
                packet,
                ENetworkEventSource.CLIENT);
            
            return true;
        }

        #endregion
        
        #region Ping handling

        private void OnPing()
        {
            //Created entities
            if (isServer)
            {
                foreach (var newNetworkEntityID in newNetworkEntities)
                {
                    if (!networkIDToEntityIDMap.TryGetRight(
                        newNetworkEntityID,
                        out var entityID))
                    {
                        continue;
                    }

                    if (!entityManager.HasEntity(entityID))
                    {
                        continue;
                    }

                    var registryEntity = entityManager.GetRegistryEntity(
                        entityID);
                    
                    if (!registryEntity.Has<PrototypeInstanceComponent>())
                    {
                        continue;
                    }

                    if (!TryGetUShortPrototypeID(
                        registryEntity.Get<PrototypeInstanceComponent>().PrototypeID,
                        out var prototypeIDAsUShort))
                    {
                        continue;
                    }
                    
                    byte authoringPermission = byte.MaxValue;

                    if (registryEntity.Has<AuthoringPermissionComponent>())
                    {
                        authoringPermission = registryEntity.Get<AuthoringPermissionComponent>().PlayerSlot;
                    }
                    
                    var simulationEntity = entityManager.GetEntity(
                        entityID,
                        relevantWorldID);

                    ushort count = entityComponentSerializerManager.ParseEntity(
                        simulationEntity,
                        out var serializerIDs,
                        out var serializers);
                    
                    networkBusAsSender
                        .PopMessage<ServerSendPacketMessage>(
                            out var sendPacketMessage)
                        .Write<ServerSendPacketMessage>(
                            sendPacketMessage,
                            new object[]
                            {
                                byte.MaxValue,
                                typeof(EntityCreatedPacket),
                                new EntityCreatedPacket
                                {
                                    ServerTick = networkHost.Tick,
                            
                                    NetworkID = newNetworkEntityID,
                                    
                                    AuthoringPermission = authoringPermission,
                            
                                    GUID = entityID.ToByteArray(),
                            
                                    PrototypeID = prototypeIDAsUShort,
                            
                            
                                    ComponentDataCount = count,
                            
                                    SerializerIDs = serializerIDs,
                            
                                    Serializers = serializers
                                },
                                entityPacketSerializationAction,
                                DeliveryMethod.ReliableUnordered
                            })
                        .SendImmediately<ServerSendPacketMessage>(
                            sendPacketMessage);
                }
                
                newNetworkEntities.Clear();
            }

            //Roll calls
            if (isServer)
            {
                rollCallCountdown--;

                if (rollCallCountdown <= 0)
                {
                    rollCallCountdown = networkRollCallSettings.RollCallFrequency;
                    
                    var networkIDs = networkIDToEntityIDMap.LeftValues.ToArray();

                    networkBusAsSender
                        .PopMessage<ServerSendPacketMessage>(
                            out var sendPacketMessage)
                        .Write<ServerSendPacketMessage>(
                            sendPacketMessage,
                            new object[]
                            {
                                byte.MaxValue,
                                typeof(NetworkEntityRollCallPacket),
                                new NetworkEntityRollCallPacket
                                {
                                    ServerTick = networkHost.Tick,

                                    NetworkIDs = networkIDs
                                },
                                null,
                                DeliveryMethod.ReliableUnordered
                            })
                        .SendImmediately<ServerSendPacketMessage>(
                            sendPacketMessage);
                }
            }

            //Events
            if (eventPacketCache.Count > 0)
            {
                if (isClient)
                    SendEventsToServer();

                if (isServer)
                    SendEventsToClients();
                
                eventPacketCache.Clear();
            }
        }

        #endregion

        private bool ParseEventEntity(
            Entity eventEntity,
            out EventPacket eventPacket)
        {
            eventPacket = null;

            ushort count = eventComponentSerializerManager.ParseEntity(
                eventEntity,
                out var serializerIDs,
                out var serializers);

            if (count == 0)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"EVENT ENTITY HAS NO COMPONENTS, ABORTING SENDING");
                
                return false;
            }
            
            if (serializerIDs == null)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"serializerIDs IS NULL");
                
                return false;
            }

            if (serializers == null)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"serializers IS NULL");
                
                return false;
            }
            
            if (playerSettings == null)
            {
                logger?.LogError<NetworkEntityManager>(
                    $"playerSettings IS NULL");
                
                return false;
            }
            
            eventPacket = new EventPacket
            {
                PlayerSlot = isServer
                    ? byte.MaxValue
                    : playerSettings.PlayerSlot,
                
                Count = count,

                SerializerIDs = serializerIDs,
                
                Serializers = serializers
            };

            if (logger != null)
            {
                bool spam = false;
                
                StringBuilder sb = new StringBuilder("SENDING EVENT. COMPONENTS: [ ");

                var entitySerializationWrapper = new EntitySerializationWrapper(eventEntity);

                foreach (var component in entitySerializationWrapper.Components)
                {
                    sb.Append($"{component.Type.Name}, ");
                    
                    //if (//component.Type == typeof(OrderReceivedEventComponent)
                    //    component.Type == typeof(TeamTagChangedEventComponent)
                    //    || component.Type == typeof(DamageDealtEventComponent))
                    //    spam = true;
                }

                sb.Remove(
                    sb.Length - 2,
                    2);

                sb.Append(" ]");

                if (!spam)
                {
                    logger?.Log<NetworkEntityManager>(
                        sb.ToString());
                }
            }

            return true;
        }
        
        private void ParseEventPacket(
            EventPacket eventPacket,
            ENetworkEventSource source)
        {
            eventEntityBuilder.NewEvent(
                out var eventEntity);

            eventComponentSerializerManager.PopulateEntity(
                eventEntity,
                eventPacket.Serializers,
                eventPacket.SerializerIDs);
            
            eventEntityBuilder.WithData<NetworkEventSourceComponent>(
                eventEntity,
                new NetworkEventSourceComponent
                {
                    Source = source,
                    
                    PlayerSlot = eventPacket.PlayerSlot
                });

            if (logger != null)
            {
                bool spam = false;
                
                StringBuilder sb = new StringBuilder("RECEIVED EVENT. COMPONENTS: [ ");

                var entitySerializationWrapper = new EntitySerializationWrapper(eventEntity);

                foreach (var component in entitySerializationWrapper.Components)
                {
                    sb.Append($"{component.Type.Name}, ");
                    
                    //if (//component.Type == typeof(OrderReceivedEventComponent)
                    //    component.Type == typeof(TeamTagChangedEventComponent)
                    //    || component.Type == typeof(DamageDealtEventComponent))
                    //    spam = true;
                }

                sb.Remove(
                    sb.Length - 2,
                    2);

                sb.Append(" ]");

                if (!spam)
                {
                    logger?.Log<NetworkEntityManager>(
                        sb.ToString());
                }
            }
        }
        
        private void SendEventsToServer()
        {
            networkBusAsSender
                .PopMessage<ClientSendPacketMessage>(
                    out var sendPacketMessage)
                .Write<ClientSendPacketMessage>(
                    sendPacketMessage,
                    new object[]
                    {
                        typeof(EventBatchPacket),
                        new EventBatchPacket
                        {
                            ServerTick = 0, //I'M A ~TEAPOT~ CLIENT, I HAVE NO SERVER TICKS
                            
                            Count = (ushort)eventPacketCache.Count,
                            
                            Events = eventPacketCache.ToArray()
                        },
                        eventPacketSerializationAction,
                        DeliveryMethod.ReliableUnordered
                    })
                .SendImmediately<ClientSendPacketMessage>(
                    sendPacketMessage);
        }

        private void SendEventsToClients()
        {
            networkBusAsSender
                .PopMessage<ServerSendPacketMessage>(
                    out var sendPacketMessage)
                .Write<ServerSendPacketMessage>(
                    sendPacketMessage,
                    new object[]
                    {
                        byte.MaxValue,
                        typeof(EventBatchPacket),
                        new EventBatchPacket
                        {
                            ServerTick = networkHost.Tick,
                            
                            Count = (ushort)eventPacketCache.Count,
                            
                            Events = eventPacketCache.ToArray()
                        },
                        eventPacketSerializationAction,
                        DeliveryMethod.ReliableUnordered
                    })
                .SendImmediately<ServerSendPacketMessage>(
                    sendPacketMessage);
        }
    }
    */
}