using System;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;

using HereticalSolutions.Logging;

using LiteNetLib;
using LiteNetLib.Utils;

namespace HereticalSolutions.Networking.LiteNetLib
{
    public class NetworkHost
        : INetworkHost,
          INetEventListener
    {
        private readonly NetworkBasicSettings basicSettings;

        private readonly NetworkDefaultConnectionValuesSettings defaultConnectionValuesSettings;

        private readonly NetworkTickSettings tickSettings;


        private readonly INonAllocSubscribableNoArgs pinger;
        
        #region Message bus

        private readonly INonAllocMessageSender networkBusAsSender;
        
        private readonly INonAllocMessageReceiver networkBusAsReceiver;

        #endregion
        
        private readonly IPacketRepository packetRepository;
        
        private readonly NetDataWriter writer;
        
        private readonly NetPacketProcessor packetProcessor;
        

        private readonly ILogger logger;
        
        
        private EHostStatus status = EHostStatus.OFFLINE;

        private int activeConnectionsCount = 0;

        private ServerToClientConnectionDescriptor[] connections;

        private ushort serverTick;
        
        
        private NetManager netManager;

        public NetManager NetManager
        {
            get => netManager;
            set => netManager = value;
        }

        public NetPeer NetPeer { get; private set; }

        #region Subscriptions

        private ISubscription pingSubscription;
        
        private ISubscription startServerSubscription;
        
        private ISubscription stopServerSubscription;
        
        private ISubscription sendPacketSubscription;
        
        #endregion

        
        public NetworkHost(
            NetworkBasicSettings basicSettings,
            NetworkDefaultConnectionValuesSettings defaultConnectionValuesSettings,
            NetworkTickSettings tickSettings,

            INonAllocSubscribableNoArgs pinger,
            
            INonAllocMessageSender networkBusAsSender,
            INonAllocMessageReceiver networkBusAsReceiver,
            
            IPacketRepository packetRepository,
            
            NetDataWriter writer,
            NetPacketProcessor packetProcessor,
            
            ServerToClientConnectionDescriptor[] connections,
            
            ILogger logger = null)
        {
            this.basicSettings = basicSettings;

            this.defaultConnectionValuesSettings = defaultConnectionValuesSettings;

            this.tickSettings = tickSettings;


            this.pinger = pinger;
            
            
            this.networkBusAsSender = networkBusAsSender;
            
            this.networkBusAsReceiver = networkBusAsReceiver;
            
            
            this.packetRepository = packetRepository;
            
            
            this.writer = writer;
            
            this.packetProcessor = packetProcessor;
            
            this.connections = connections;
            
            
            this.logger = logger;

            
            status = EHostStatus.OFFLINE;
            
            serverTick = 0;
            
            
            packetProcessor.SubscribeReusable<JoinRequestPacket, NetPeer>(OnJoinRequestReceived);
            
            
            pingSubscription = DelegatesFactory.BuildSubscriptionNoArgs(
                OnPing);
            
            startServerSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ServerStartMessage>(
                OnServerStartMessage);
            
            stopServerSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ServerStopMessage>(
                OnServerStopMessage);
            
            sendPacketSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<ServerSendPacketMessage>(
                OnServerSendPacketMessage);
            
            
            networkBusAsReceiver.SubscribeTo<ServerStartMessage>(
                startServerSubscription);
            
            networkBusAsReceiver.SubscribeTo<ServerStopMessage>(
                stopServerSubscription);
            
            networkBusAsReceiver.SubscribeTo<ServerSendPacketMessage>(
                sendPacketSubscription);
            
        }

        #region INetworkHost

        public EHostStatus Status => status;

        public ushort Tick => serverTick;
        
        public int ActiveConnectionsCount => activeConnectionsCount;

        public ServerToClientConnectionDescriptor[] Connections => connections;

        public async Task Start(
            int port,
            bool reserveSlotForSelf = false)
        {
            if (status != EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING START: STATUS {status} IS NOT {EHostStatus.OFFLINE}");
                
                return;
            }
            
            netManager.Start(port);
            
            if (pingSubscription.Active)
            {
                logger?.LogError<NetworkHost>(
                    $"PING SUBSCRIPTION IS STILL ACTIVE");
                
                (pingSubscription as ISubscriptionHandler<INonAllocSubscribableNoArgs, IInvokableNoArgs>).Terminate();
            }
            
            pinger.Subscribe(
                pingSubscription);

            for (int i = 0; i < connections.Length; i++)
            {
                connections[i] = default;

                connections[i].PeerID = -1;
            }

            if (reserveSlotForSelf)
                connections[0].Status = EServerToClientConnectionStatus.SELF;
            
            status = EHostStatus.ONLINE;
            
            logger?.Log<NetworkHost>(
                $"ONLINE");
            
            networkBusAsSender
                .PopMessage<ServerStartedMessage>(
                    out var message)
                .SendImmediately<ServerStartedMessage>(
                    message);
        }

        public async Task Stop()
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING STOP: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            status = EHostStatus.OFFLINE;
            
            if (pingSubscription.Active)
            {
                pinger.Unsubscribe(
                    pingSubscription);
            }
            
            if (sendPacketSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ServerSendPacketMessage>(
                    sendPacketSubscription);
            }
            
            if (stopServerSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ServerStopMessage>(
                    stopServerSubscription);
            }
            
            if (startServerSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ServerStartMessage>(
                    startServerSubscription);
            }
            
            netManager.Stop();

            for (int i = 0; i < connections.Length; i++)
            {
                connections[i] = default;

                connections[i].PeerID = -1;
            }

            logger?.Log<NetworkHost>(
                $"OFFLINE");
            
            networkBusAsSender
                .PopMessage<ServerStoppedMessage>(
                    out var message)
                .SendImmediately<ServerStoppedMessage>(
                    message);
        }

        #endregion

        #region INetEventListener

        public void OnPeerConnected(NetPeer peer)
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING OnPeerConnected: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            IPEndPoint endPoint = peer as IPEndPoint;
            
            NetPeer = peer;
            
            logger?.Log<NetworkHost>(
                $"PEER CONNECTED: {endPoint}");
        }

        public void OnPeerDisconnected(
            NetPeer peer,
            DisconnectInfo disconnectInfo)
        {
            logger?.Log<NetworkHost>(
                $"PEER DISCONNECTED. REASON: {disconnectInfo.Reason}");
            
            byte playerSlot = byte.MaxValue;
            
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].PeerID == peer.Id)
                {
                    playerSlot = (byte)i;
                    
                    break;
                }
            }

            if (playerSlot != byte.MaxValue)
            {
                connections[playerSlot].Status = EServerToClientConnectionStatus.DISCONNECTED;
            }
            
            networkBusAsSender
                .PopMessage<ServerClientDisconnectedMessage>(
                    out var message)
                .Write<ServerClientDisconnectedMessage>(
                    message,
                    new object[]
                    {
                    })
                .SendImmediately<ServerClientDisconnectedMessage>(
                    message);
        }

        public void OnNetworkError(
            IPEndPoint endPoint,
            SocketError socketError)
        {
            logger?.LogError<NetworkHost>(
                $"NETWORK ERROR: {socketError}");
        }

        public void OnNetworkReceive(
            NetPeer peer,
            NetPacketReader reader,
            byte channelNumber,
            DeliveryMethod deliveryMethod)
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING OnNetworkReceive: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }

            byte playerSlot = byte.MaxValue;
            
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].PeerID == peer.Id)
                {
                    playerSlot = (byte)i;
                    
                    break;
                }
            }
            
            byte packetID = reader.GetByte();
            
            if (!packetRepository.TryGetPacketType(
                packetID,
                out Type packetType))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET TYPE FOR ID: {packetID}"));
            }

            logger?.Log<NetworkHost>(
                $"RECEIVED PACKET: {packetType.Name}");

            if (packetType == typeof(JoinRequestPacket))
            {
                /*
                packetProcessor.ReadAllPackets(
                    reader,
                    peer);
                */

                var joinRequestPacket = new JoinRequestPacket();
                
                joinRequestPacket.Deserialize(reader);
                
                OnJoinRequestReceived(
                    joinRequestPacket,
                    peer);
                
                return;
            }

            //packetProcessor.ReadAllPackets(
            //    reader,
            //    peer);

            networkBusAsSender
                .PopMessage<ServerReceivedPacketMessage>(
                    out var message)
                .Write<ServerReceivedPacketMessage>(
                    message,
                    new object[]
                    {
                        playerSlot,
                        peer,
                        
                        packetType,
                        reader,
                        
                        channelNumber,
                        deliveryMethod
                    })
                .SendImmediately<ServerReceivedPacketMessage>(
                    message);
        }

        public void OnNetworkReceiveUnconnected(
            IPEndPoint remoteEndPoint,
            NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            logger?.LogError<NetworkHost>(
                $"RECEIVED PACKET FROM UNCONNECTED PEER. MESSAGE TYPE: {messageType}");
        }

        public void OnNetworkLatencyUpdate(
            NetPeer peer,
            int latency)
        {
            var index = peer.Id;

            connections[index].Ping = latency;
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING OnConnectionRequest: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            request.AcceptIfKey(basicSettings.Secret);
        }

        #endregion
        
        #region Packet handlers
        
        private void OnJoinRequestReceived(
            JoinRequestPacket packet,
            NetPeer peer)
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING OnJoinRequestReceived: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }

            logger?.Log<NetworkHost>(
                $"JOIN REQUEST PACKET RECEIVED. USERNAME: {packet.Username} PREFERRED PLAYER SLOT: {packet.PreferredPlayerSlot}");
            
            byte playerSlot = packet.PreferredPlayerSlot;

            if (playerSlot != byte.MaxValue
                && connections[playerSlot].Status != EServerToClientConnectionStatus.DISCONNECTED)
            {
                logger?.Log<NetworkHost>(
                    $"PREFERRED SLOT {playerSlot} IS NOT AVAILABLE. FINDING AVAILABLE SLOT");

                playerSlot = byte.MaxValue;
            }

            if (playerSlot == byte.MaxValue)
            {
                for (int i = 0; i < connections.Length; i++)
                {
                    if (connections[i].Status == EServerToClientConnectionStatus.DISCONNECTED)
                    {
                        playerSlot = (byte)i;
                        
                        break;
                    }
                }
            }

            connections[playerSlot].Status = EServerToClientConnectionStatus.CONNECTED;
            
            connections[playerSlot].PeerID = peer.Id;
            
            
            var peerAsEndPoint = peer as IPEndPoint;
            
            connections[playerSlot].ClientIP = peerAsEndPoint.Address.ToString();
            
            connections[playerSlot].ClientPort = peerAsEndPoint.Port;
            
            connections[playerSlot].Ping = peer.Ping;
            
            
            connections[playerSlot].Username = packet.Username;
            
            
            //Send join accept
            var joinConfirmedPacket = new JoinConfirmedPacket
            {
                PlayerSlot = playerSlot,
                
                ServerTick = (ushort)serverTick
            };
            
            SendPacketSerializable<JoinConfirmedPacket>(
                playerSlot,
                joinConfirmedPacket,
                DeliveryMethod.ReliableOrdered);
            
            /*
            peer.Send(
                WritePacket(joinConfirmedPacket),
                DeliveryMethod.ReliableOrdered);
            */

            //Send to old players info about new player
            var playerJoinedPacket = new PlayerJoinedPacket
            {
                PlayerSlot = playerSlot,
                
                Username = connections[playerSlot].Username,
                
                Rejoin = false,
                
                ServerTick = serverTick
            };
            
            /*
            netManager.SendToAll(
                WritePacket(playerJoinedPacket),
                DeliveryMethod.ReliableOrdered,
                peer);
            */
            
            SendPacketSerializable<PlayerJoinedPacket>(
                byte.MaxValue,
                playerJoinedPacket,
                DeliveryMethod.ReliableOrdered);

            //Send to new player info about old players
            for (int i = 0; i < connections.Length; i++)
            {
                if (i == playerSlot)
                    continue;
                
                if (connections[i].Status != EServerToClientConnectionStatus.CONNECTED)
                    continue;
                
                var playerJoinedPacketForNewUser = new PlayerJoinedPacket
                {
                    PlayerSlot = (byte)i,
                
                    Username = connections[i].Username,
                
                    Rejoin = false,
                
                    ServerTick = serverTick
                };
                
                /*
                peer.Send(
                    WritePacket(playerJoinedPacketForNewUser),
                    DeliveryMethod.ReliableOrdered);
                */
                
                SendPacketSerializable<PlayerJoinedPacket>(
                    playerSlot,
                    playerJoinedPacketForNewUser,
                    DeliveryMethod.ReliableOrdered);
            }
            
            networkBusAsSender
                .PopMessage<ServerClientJoinedMessage>(
                    out var message)
                .Write<ServerClientJoinedMessage>(
                    message,
                    new object[]
                    {
                    })
                .SendImmediately<ServerClientJoinedMessage>(
                    message);
            
            logger?.Log<NetworkHost>(
                $"PLAYER JOINED. Username: {connections[playerSlot].Username} Player slot: {playerSlot}");
        }
        
        #endregion
        
        #region Message handlers

        private void OnServerStartMessage(ServerStartMessage message)
        {
            int port = message.Port;
            
            if (port <= 0)
                port = defaultConnectionValuesSettings.DefaultPort;
            
            Start(
                port,
                message.ReserveSlotForSelf);
        }
        
        private void OnServerStopMessage(ServerStopMessage message)
        {
            Stop();
        }
        
        private void OnServerSendPacketMessage(ServerSendPacketMessage message)
        {
            if (message.PacketSerializationAction == null)
            {
                SendPacketSerializable(
                    message.PlayerSlot,
                    message.PacketType,
                    message.Packet,
                    message.DeliveryMethod);
            }
            else
            {
                SendPacketSerializable(
                    message.PlayerSlot,
                    message.PacketType,
                    message.Packet,
                    message.PacketSerializationAction,
                    message.DeliveryMethod);
            }
        }

        #endregion

        #region Ping handling

        private void OnPing()
        {
            serverTick = (ushort)((serverTick + 1) % tickSettings.MaxTickValue);
            
            netManager.PollEvents();
        }

        #endregion
        
        #region Write packet

        private NetDataWriter WriteSerializable<T>(T packet) where T : struct, INetSerializable
        {
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                typeof(T),
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {typeof(T).Name}"));
            }

            writer.Put(
                packetID);
            
            packet.Serialize(writer);
            
            return writer;
        }

        private NetDataWriter WritePacket<T>(T packet) where T : class, new()
        {
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                typeof(T),
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {typeof(T).Name}"));
            }

            writer.Put(
                packetID);
            
            packetProcessor.Write(
                writer,
                packet);
            
            return writer;
        }

        #endregion
        
        #region Send packet
        
        private void SendPacket<T>(
            byte playerSlot,
            T packet,
            DeliveryMethod deliveryMethod)
            where T : class, new()
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING SendPacket: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            writer.Reset();

            if (!packetRepository.TryGetPacketID(
                typeof(T),
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {typeof(T).Name}"));
            }

            writer.Put(
                packetID);
            
            packetProcessor.Write(
                writer,
                packet);

            if (playerSlot != byte.MaxValue)
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {typeof(T).Name} TO PLAYER SLOT: {playerSlot} PEER: {connections[playerSlot].PeerID}");
                
                netManager
                    .GetPeerById(connections[playerSlot].PeerID)
                    .Send(
                        writer,
                        deliveryMethod);
            }
            else
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {typeof(T).Name} TO ALL CONNECTIONS");
                
                netManager.SendToAll(
                    writer,
                    deliveryMethod);
            }
        }
        
        private void SendPacket(
            byte playerSlot,
            Type packetType,
            object packet,
            DeliveryMethod deliveryMethod)
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING SendPacket: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            writer.Reset();

            if (!packetRepository.TryGetPacketID(
                packetType,
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {packetType.Name}"));
            }

            writer.Put(
                packetID);
            
            packetProcessor.Write(
                writer,
                packet);
            
            if (playerSlot != byte.MaxValue)
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {packetType.Name} TO PLAYER SLOT: {playerSlot} PEER: {connections[playerSlot].PeerID}");
                
                netManager
                    .GetPeerById(connections[playerSlot].PeerID)
                    .Send(
                        writer,
                        deliveryMethod);
            }
            else
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {packetType.Name} TO ALL CONNECTIONS");
                
                netManager.SendToAll(
                    writer,
                    deliveryMethod);
            }
        }
        
        private void SendPacketSerializable<T>(
            byte playerSlot,
            T packet,
            DeliveryMethod deliveryMethod)
            where T : INetSerializable
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING SendPacketSerializable: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                typeof(T),
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {typeof(T).Name}"));
            }

            //writer.Put(packetType);
            writer.Put(
                packetID);
            
            packet.Serialize(writer);
            
            if (playerSlot != byte.MaxValue)
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {typeof(T).Name} TO PLAYER SLOT: {playerSlot} PEER: {connections[playerSlot].PeerID}");
                
                netManager
                    .GetPeerById(connections[playerSlot].PeerID)
                    .Send(
                        writer,
                        deliveryMethod);
            }
            else
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {typeof(T).Name} TO ALL CONNECTIONS");
                
                netManager.SendToAll(
                    writer,
                    deliveryMethod);
            }
        }
        
        private void SendPacketSerializable(
            byte playerSlot,
            Type packetType,
            INetSerializable packet,
            DeliveryMethod deliveryMethod)
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING SendPacketSerializable: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                packetType,
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {packetType.Name}"));
            }

            //writer.Put(packetType);
            writer.Put(
                packetID);
            
            packet.Serialize(writer);
            
            if (playerSlot != byte.MaxValue)
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {packetType.Name} TO PLAYER SLOT: {playerSlot} PEER: {connections[playerSlot].PeerID}");
                
                netManager
                    .GetPeerById(connections[playerSlot].PeerID)
                    .Send(
                        writer,
                        deliveryMethod);
            }
            else
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {packetType.Name} TO ALL CONNECTIONS");
                
                netManager.SendToAll(
                    writer,
                    deliveryMethod);
            }
        }
        
        private void SendPacketSerializable(
            byte playerSlot,
            Type packetType,
            INetSerializable packet,
            Action<INetSerializable, NetDataWriter> packetSerializationAction,
            DeliveryMethod deliveryMethod)
        {
            if (status == EHostStatus.OFFLINE)
            {
                logger?.LogError<NetworkHost>(
                    $"ABORTING SendPacketSerializable: STATUS IS {EHostStatus.OFFLINE}");
                
                return;
            }
            
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                packetType,
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkHost>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {packetType.Name}"));
            }

            writer.Put(
                packetID);
            
            packetSerializationAction?.Invoke(
                packet,
                writer);
            
            if (playerSlot != byte.MaxValue)
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {packetType.Name} TO PLAYER SLOT: {playerSlot} PEER: {connections[playerSlot].PeerID}");
                
                netManager
                    .GetPeerById(connections[playerSlot].PeerID)
                    .Send(
                        writer,
                        deliveryMethod);
            }
            else
            {
                logger?.Log<NetworkHost>(
                    $"SENDING PACKET: {packetType.Name} TO ALL CONNECTIONS");
                
                netManager.SendToAll(
                    writer,
                    deliveryMethod);
            }
        }
        
        #endregion
    }
}