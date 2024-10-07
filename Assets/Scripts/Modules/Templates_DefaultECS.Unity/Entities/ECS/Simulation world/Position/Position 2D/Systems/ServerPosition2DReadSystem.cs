using System.Collections.Generic;

using HereticalSolutions.Time;

using HereticalSolutions.Messaging;

using HereticalSolutions.Networking;

using HereticalSolutions.Templates.Universal.Networking;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerPosition2DReadSystem : AServerReadSystem<
        Position2DComponent,
        ServerPosition2DComponent,
        IDPosition2DPair,
        EntityPosition2DDeltasPacket>
    {
        public ServerPosition2DReadSystem(
            World world,
            NetworkDeltaReplicationSettings networkDeltaReplicationSettings,
            ITimerManager simulationTimerManager,
            INetworkHost host,
            UniversalTemplateEntityManager entityManager,
            INonAllocMessageSender networkBusAsSender,
            List<IDPosition2DPair> dirtyEntities,
            ILoggerResolver loggerResolver,
            ILogger logger = null,
            float controlPacketFrequencyChance = DEFAULT_CONTROL_PACKET_FREQUENCY_CHANCE)
            : base(
                world,
                networkDeltaReplicationSettings,
                simulationTimerManager,
                host,
                entityManager,
                networkBusAsSender,
                dirtyEntities,
                loggerResolver,
                logger,
                controlPacketFrequencyChance)
        {
        }

        protected override bool ComponentChanged(
            Position2DComponent position2DComponent,
            ServerPosition2DComponent serverPosition2DComponent)
        {
            return (position2DComponent.Position - serverPosition2DComponent.ServerPosition).sqrMagnitude
               > MathHelpers.EPSILON;
        }

        protected override void ReadComponentToServerComponent(
            Position2DComponent position2DComponent,
            ref ServerPosition2DComponent serverPosition2DComponent,
            ushort tick)
        {
            serverPosition2DComponent.ServerPosition = position2DComponent.Position;

            serverPosition2DComponent.ServerTick = tick;
        }

        protected override IDPosition2DPair CreateIDValuePair(
            ushort networkID,
            Position2DComponent position2DComponent)
        {
            return new IDPosition2DPair
            {
                NetworkID = networkID,

                Position = position2DComponent.Position 
            };
        }

        protected override int GetMaxCapacityPerPacket()
        {
            return EntityPosition2DDeltasPacket.MAX_CAPACITY;
        }

        protected override EntityPosition2DDeltasPacket CreateDeltaPacket(
            ushort tick,
            ushort count,
            IDPosition2DPair[] payload)
        {
            return new EntityPosition2DDeltasPacket
            {
                ServerTick = host.Tick,

                Count = (ushort)payload.Length,

                Positions = payload
            };
        }
    }
}