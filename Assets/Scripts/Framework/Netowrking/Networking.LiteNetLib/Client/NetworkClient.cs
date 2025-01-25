using System;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Messaging;

using HereticalSolutions.Logging;

using LiteNetLib;
using LiteNetLib.Utils;

using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.Networking.LiteNetLib
{
    public class NetworkClient
        : INetworkClient,
          INetEventListener
    {
        private readonly NetworkPlayerSettings playerSettings;
        
        private readonly INonAllocSubscribable pinger;
        
        #region Message bus

        private readonly INonAllocMessageSender networkBusAsSender;
        
        private readonly INonAllocMessageReceiver networkBusAsReceiver;
        
        
        private readonly INonAllocMessageSender gameStateBusAsSender;

        #endregion
        
        private readonly IPacketRepository packetRepository;
        
        private readonly NetDataWriter writer;
        
        private readonly NetPacketProcessor packetProcessor;

        private readonly ILogger logger;
        
        
        private EClientStatus status = EClientStatus.DISCONNECTED;

        private ClientToServerConnectionDescriptor connection;

        private NetPeer server;
        
        private NetManager netManager;

        public NetManager NetManager
        {
            get => netManager;
            set => netManager = value;
        }

        public NetPeer NetPeer => server;

        #region Subscriptions

        private INonAllocSubscription pingSubscription;
        
        private INonAllocSubscription startClientSubscription;
        
        private INonAllocSubscription stopClientSubscription;
        
        private INonAllocSubscription connectSubscription;
        
        private INonAllocSubscription disconnectSubscription;

        private INonAllocSubscription sendPacketSubscription;
        
        #endregion
        
        public NetworkClient(
            NetworkPlayerSettings playerSettings,
            
            INonAllocSubscribable pinger,
            
            INonAllocMessageSender networkBusAsSender,
            INonAllocMessageReceiver networkBusAsReceiver,
            INonAllocMessageSender gameStateBusAsSender,
            
            IPacketRepository packetRepository,
            
            NetDataWriter writer,
            NetPacketProcessor packetProcessor,
            
            ILogger logger)
        {
            this.playerSettings = playerSettings;
            
            
            this.pinger = pinger;
            
            
            this.networkBusAsSender = networkBusAsSender;
            
            this.networkBusAsReceiver = networkBusAsReceiver;

            this.gameStateBusAsSender = gameStateBusAsSender;
            
            
            this.packetRepository = packetRepository;
            
            
            this.writer = writer;
            
            this.packetProcessor = packetProcessor;
            
            
            this.logger = logger;

            
            status = EClientStatus.DISCONNECTED;
            
            server = null;

            connection = default;
            
            
            packetProcessor.SubscribeReusable<PlayerJoinedPacket>(OnPlayerJoined);
            
            packetProcessor.SubscribeReusable<JoinConfirmedPacket>(OnJoinConfirmed);
            
            packetProcessor.SubscribeReusable<PlayerLeftPacket>(OnPlayerLeft);
            
            packetProcessor.SubscribeReusable<GameStartedPacket>(OnGameStarted);


            pingSubscription = DelegateWrapperFactory.BuildSubscriptionNoArgs(
                OnPing);
            
            startClientSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<ClientStartMessage>(
                OnClientStartMessage);
            
            stopClientSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<ClientStopMessage>(
                OnClientStopMessage);
            
            connectSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<ClientConnectMessage>(
                OnClientConnectMessage);
            
            disconnectSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<ClientDisconnectMessage>(
                OnClientDisconnectMessage);
            
            sendPacketSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<ClientSendPacketMessage>(
                OnClientSendPacketMessage);

            
            networkBusAsReceiver.SubscribeTo<ClientStartMessage>(
                startClientSubscription);
            
            networkBusAsReceiver.SubscribeTo<ClientStopMessage>(
                stopClientSubscription);
            
            networkBusAsReceiver.SubscribeTo<ClientConnectMessage>(
                connectSubscription);
            
            networkBusAsReceiver.SubscribeTo<ClientDisconnectMessage>(
                disconnectSubscription);
            
            networkBusAsReceiver.SubscribeTo<ClientSendPacketMessage>(
                sendPacketSubscription);
        }

        #region INetworkClient

        public EClientStatus Status => status;

        public ClientToServerConnectionDescriptor Connection => connection;

        public void Start()
        {
            if (status != EClientStatus.DISCONNECTED)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING START: STATUS {status} IS NOT {EClientStatus.DISCONNECTED}");
                
                return;
            }

            netManager.Start();
            
            if (pingSubscription.Active)
            {
                logger?.LogError<NetworkClient>(
                    $"PING SUBSCRIPTION IS STILL ACTIVE");

                pingSubscription.Unsubscribe();
            }
            
            pinger.Subscribe(
                pingSubscription);
            
            logger?.Log<NetworkClient>(
                $"STARTED");
            
            networkBusAsSender
                .PopMessage<ClientStartedMessage>(
                    out var message)
                .SendImmediately<ClientStartedMessage>(
                    message);
        }

        public void Stop()
        {
            if (status == EClientStatus.DISCONNECTED)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING STOP: STATUS IS {EClientStatus.DISCONNECTED}");
                
                return;
            }

            if (pingSubscription.Active)
            {
                pinger.Unsubscribe(
                    pingSubscription);
            }
            
            if (startClientSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ClientStartMessage>(
                    startClientSubscription);
            }
            
            if (stopClientSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ClientStopMessage>(
                    stopClientSubscription);
            }
            
            if (connectSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ClientConnectMessage>(
                    connectSubscription);
            }
            
            if (disconnectSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ClientDisconnectMessage>(
                    disconnectSubscription);
            }
            
            if (sendPacketSubscription.Active)
            {
                networkBusAsReceiver.UnsubscribeFrom<ClientSendPacketMessage>(
                    sendPacketSubscription);
            }
            
            netManager.Stop();
            
            logger?.Log<NetworkClient>(
                $"STOPPED");
            
            networkBusAsSender
                .PopMessage<ClientStoppedMessage>(
                    out var message)
                .SendImmediately<ClientStoppedMessage>(
                    message);
        }

        public async Task Connect(
            string ip,
            int port,
            string secret,
            
            //Async tail
            AsyncExecutionContext asyncContext,

            byte preferredPlayerSlot = byte.MaxValue)
        {
            if (status != EClientStatus.DISCONNECTED)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING CONNECT: STATUS {status} IS NOT {EClientStatus.DISCONNECTED}");
                
                return;
            }

            status = EClientStatus.CONNECTING;
            
            //Just storing it for later use
            connection.PlayerSlot = preferredPlayerSlot;
            
            netManager.Connect(
                ip,
                port,
                secret);
            
            /*
            while (status != EClientStatus.CONNECTED
                && status != EClientStatus.DISCONNECTED)
            {
                await Task.Yield();
            }
            */
            
            logger?.Log<NetworkClient>(
                $"CONNECTED");
            
            networkBusAsSender
                .PopMessage<ClientConnectedMessage>(
                    out var message)
                .SendImmediately<ClientConnectedMessage>(
                    message);
        }

        public void Disconnect()
        {
            if (status == EClientStatus.DISCONNECTED)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING DISCONNECT: STATUS IS {EClientStatus.DISCONNECTED}");
                
                return;
            }
            
            status = EClientStatus.DISCONNECTED;

            connection = default;
            
            server = null;
            
            netManager.Stop();
            
            logger?.Log<NetworkClient>(
                $"DISCONNECTED");
            
            networkBusAsSender
                .PopMessage<ClientDisconnectedMessage>(
                    out var message)
                .SendImmediately<ClientDisconnectedMessage>(
                    message);
        }

        #endregion

        #region INetEventListener

        public void OnPeerConnected(NetPeer peer)
        {
            if (status != EClientStatus.CONNECTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING OnPeerConnected: STATUS {status} IS NOT {EClientStatus.CONNECTING}");
                
                return;
            }
            
            IPEndPoint endPoint = peer as IPEndPoint;
            
            logger?.Log<NetworkClient>(
                $"CONNECTED TO SERVER: {endPoint}");
            
            status = EClientStatus.REQUESTING;

            connection.ServerIP = endPoint.Address.ToString();
            
            connection.ServerPort = endPoint.Port;
            
            server = peer;
            
            logger?.Log<NetworkClient>(
                $"REQUESTING TO JOIN");
            
            SendPacketSerializable<JoinRequestPacket>(
                new JoinRequestPacket
                {
                    Username = playerSettings.PlayerName,
                    
                    PlayerId = playerSettings.PlayerId,
                    
                    PreferredPlayerSlot = connection.PlayerSlot
                },
                DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerDisconnected(
            NetPeer peer,
            DisconnectInfo disconnectInfo)
        {
            status = EClientStatus.DISCONNECTED;

            connection = default;
            
            server = null;
            
            netManager.Stop();
            
            logger?.Log<NetworkClient>(
                $"DISCONNECTED FROM SERVER. REASON: {disconnectInfo.Reason}");
        }

        public void OnNetworkError(
            IPEndPoint endPoint,
            SocketError socketError)
        {
            logger?.LogError<NetworkClient>(
                $"NETWORK ERROR: {socketError}");
        }

        public void OnNetworkReceive(
            NetPeer peer,
            NetPacketReader reader,
            byte channelNumber,
            DeliveryMethod deliveryMethod)
        {
            if (status != EClientStatus.CONNECTED
                && status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING OnNetworkReceive: STATUS {status} IS NOT {EClientStatus.CONNECTED} OR {EClientStatus.REQUESTING}");

                return;
            }

            byte packetID = reader.GetByte();

            if (!packetRepository.TryGetPacketType(
                    packetID,
                    out Type packetType))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkClient>(
                        $"COULD NOT FIND PACKET TYPE FOR ID: {packetID}"));
            }

            //Just to avoid the spam
            if (logger != null)
                //&& !packetType.Name.Contains("Delta")
                //&& !packetType.Name.Contains("RollCall")
                //&& !packetType.Name.Contains("EventBatch"))
            {
                logger?.Log<NetworkClient>(
                    $"RECEIVED PACKET: {packetType.Name}");
            }

            /*
            if (packetType == typeof(JoinConfirmedPacket)
                || packetType == typeof(PlayerJoinedPacket)
                || packetType == typeof(PlayerLeftPacket))
            {
                packetProcessor.ReadAllPackets(reader);
                
                return;
            }
            */
            
            if (packetType == typeof(JoinConfirmedPacket))
            {
                var joinConfirmedPacket = new JoinConfirmedPacket();
                
                joinConfirmedPacket.Deserialize(reader);
                
                OnJoinConfirmed(
                    joinConfirmedPacket);
                
                return;
            }
            
            if (packetType == typeof(PlayerJoinedPacket))
            {
                var playerJoinedPacket = new PlayerJoinedPacket();
                
                playerJoinedPacket.Deserialize(reader);
                
                OnPlayerJoined(
                    playerJoinedPacket);
                
                return;
            }
            
            if (packetType == typeof(PlayerLeftPacket))
            {
                var playerLeftPacket = new PlayerLeftPacket();
                
                playerLeftPacket.Deserialize(reader);
                
                OnPlayerLeft(
                    playerLeftPacket);
                
                return;
            }

            if (packetType == typeof(GameStartedPacket))
            {
                var gameStartedPacket = new GameStartedPacket();
                
                gameStartedPacket.Deserialize(reader);
                
                OnGameStarted(gameStartedPacket);
                
                return;
            }

            //packetProcessor.ReadAllPackets(reader);
            
            networkBusAsSender
                .PopMessage<ClientReceivedPacketMessage>(
                    out var message)
                .Write<ClientReceivedPacketMessage>(
                    message,
                    new object[]
                    {
                        packetType,
                        reader,
                        
                        channelNumber,
                        deliveryMethod
                    })
                .SendImmediately<ClientReceivedPacketMessage>(
                    message);
        }

        public void OnNetworkReceiveUnconnected(
            IPEndPoint remoteEndPoint,
            NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            logger?.LogError<NetworkClient>(
                $"RECEIVED PACKET FROM UNCONNECTED PEER. MESSAGE TYPE: {messageType}");
        }

        public void OnNetworkLatencyUpdate(
            NetPeer peer,
            int latency)
        {
            if (status != EClientStatus.CONNECTED
                && status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING OnNetworkLatencyUpdate: STATUS {status} IS NOT {EClientStatus.CONNECTED} OR {EClientStatus.REQUESTING}");
                
                return;
            }
            
            connection.Ping = latency;
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            logger?.LogError<NetworkClient>(
                $"RECEIVED CONNECTION REQUEST: {request}");
            
            request.Reject(); //I'M A ~TEAPOT~ CLIENT, WHY WOULD I ACCEPT CONNECTIONS?
        }

        #endregion

        #region Packet handlers
        
        private void OnPlayerJoined(PlayerJoinedPacket packet)
        {
            logger?.Log<NetworkClient>(
                $"PLAYER JOINED PACKET RECEIVED. USERNAME: {packet.Username} PLAYER SLOT: {packet.PlayerSlot}");
            
            //TODO: player joined processing logic
        }
        
        private void OnPlayerLeft(PlayerLeftPacket packet)
        {
            logger?.Log<NetworkClient>(
                $"PLAYER LEFT PACKET RECEIVED. PLAYER SLOT: {packet.PlayerSlot}");
        }

        private void OnJoinConfirmed(JoinConfirmedPacket packet)
        {
            if (status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING OnJoinConfirmed: STATUS {status} IS NOT {EClientStatus.REQUESTING}");
                
                return;
            }
            
            logger?.Log<NetworkClient>(
                $"JOIN CONFIRMED PACKET RECEIVED. PLAYER SLOT: {packet.PlayerSlot}");
            
            //var serverAsEndPoint = server as IPEndPoint;
            
            connection.PlayerSlot = packet.PlayerSlot;

            connection.ServerIP = server.Address.ToString();
            
            connection.ServerPort = server.Port;
            
            status = EClientStatus.CONNECTED;
            
            playerSettings.PlayerSlot = packet.PlayerSlot;
            
            logger?.Log<NetworkClient>(
                $"JOIN CONFIRMED. SLOT: {packet.PlayerSlot}");
            
            //_lastServerTick = packet.ServerTick;
            
            networkBusAsSender
                .PopMessage<ClientJoinedServerMessage>(
                    out var message)
                .Write<ClientJoinedServerMessage>(
                    message,
                    new object[]
                    {
                    })
                .SendImmediately<ClientJoinedServerMessage>(
                    message);
        }

        private void OnGameStarted(GameStartedPacket packet)
        {
            logger?.Log<NetworkClient>(
                $"GAME STARTED PACKET RECEIVED");
            
            //TODO: game start logic
        }
        
        #endregion

        #region Message handlers

        private void OnClientStartMessage(ClientStartMessage message)
        {
            Start();
        }
        
        private void OnClientStopMessage(ClientStopMessage message)
        {
            Stop();
        }
        
        private void OnClientConnectMessage(ClientConnectMessage message)
        {
            Connect(
                message.IP,
                message.Port,
                message.Secret,
                message.PreferredPlayerSlot,
                
                null);
        }
        
        private void OnClientDisconnectMessage(ClientDisconnectMessage message)
        {
            Disconnect();
        }
        
        private void OnClientSendPacketMessage(ClientSendPacketMessage message)
        {
            if (message.PacketSerializationAction == null)
            {
                SendPacketSerializable(
                    message.PacketType,
                    message.Packet,
                    message.DeliveryMethod);
            }
            else
            {
                SendPacketSerializable(
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
            netManager.PollEvents();
        }

        #endregion
        
        #region Send packet
        
        private void SendPacket<T>(
            T packet,
            DeliveryMethod deliveryMethod)
            where T : class, new()
        {
            if (status != EClientStatus.CONNECTED
                && status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING SendPacket: STATUS {status} IS NOT {EClientStatus.CONNECTED} OR {EClientStatus.REQUESTING}");
                
                return;
            }
            
            if (server == null)
                return;
            
            writer.Reset();

            if (!packetRepository.TryGetPacketID(
                typeof(T),
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkClient>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {nameof(T)}"));
            }

            writer.Put(
                packetID);
            
            packetProcessor.Write(
                writer,
                packet);
            
            server.Send(
                writer,
                deliveryMethod);
        }
        
        private void SendPacket(
            Type packetType,
            object packet,
            DeliveryMethod deliveryMethod)
        {
            if (status != EClientStatus.CONNECTED
                && status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING SendPacket: STATUS {status} IS NOT {EClientStatus.CONNECTED} OR {EClientStatus.REQUESTING}");
                
                return;
            }
            
            if (server == null)
                return;
            
            writer.Reset();

            if (!packetRepository.TryGetPacketID(
                packetType,
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkClient>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {packetType.Name}"));
            }

            writer.Put(
                packetID);
            
            packetProcessor.Write(
                writer,
                packet);
            
            server.Send(
                writer,
                deliveryMethod);
        }
        
        private void SendPacketSerializable<T>(
            T packet,
            DeliveryMethod deliveryMethod)
            where T : INetSerializable
        {
            if (status != EClientStatus.CONNECTED
                && status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING SendPacketSerializable: STATUS {status} IS NOT {EClientStatus.CONNECTED} OR {EClientStatus.REQUESTING}");
                
                return;
            }
            
            if (server == null)
                return;
            
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                    typeof(T),
                    out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkClient>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {nameof(T)}"));
            }

            //writer.Put(packetType);
            writer.Put(
                packetID);
            
            packet.Serialize(writer);
            
            server.Send(
                writer,
                deliveryMethod);
        }
        
        private void SendPacketSerializable(
            Type packetType,
            INetSerializable packet,
            DeliveryMethod deliveryMethod)
        {
            if (status != EClientStatus.CONNECTED
                && status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING SendPacketSerializable: STATUS {status} IS NOT {EClientStatus.CONNECTED} OR {EClientStatus.REQUESTING}");
                
                return;
            }
            
            if (server == null)
                return;
            
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                packetType,
                out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkClient>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {packetType.Name}"));
            }

            //writer.Put(packetType);
            writer.Put(
                packetID);
            
            packet.Serialize(writer);
            
            server.Send(
                writer,
                deliveryMethod);
        }
        
        private void SendPacketSerializable(
            Type packetType,
            INetSerializable packet,
            Action<INetSerializable, NetDataWriter> packetSerializationAction,
            DeliveryMethod deliveryMethod)
        {
            if (status != EClientStatus.CONNECTED
                && status != EClientStatus.REQUESTING)
            {
                logger?.LogError<NetworkClient>(
                    $"ABORTING SendPacketSerializable: STATUS {status} IS NOT {EClientStatus.CONNECTED} OR {EClientStatus.REQUESTING}");
                
                return;
            }
            
            if (server == null)
                return;
            
            writer.Reset();
            
            if (!packetRepository.TryGetPacketID(
                    packetType,
                    out byte packetID))
            {
                throw new Exception(
                    logger.TryFormatException<NetworkClient>(
                        $"COULD NOT FIND PACKET ID FOR TYPE: {packetType.Name}"));
            }

            //writer.Put(packetType);
            writer.Put(
                packetID);
            
            packetSerializationAction?.Invoke(
                packet,
                writer);
            
            server.Send(
                writer,
                deliveryMethod);
        }
        
        #endregion
    }
}