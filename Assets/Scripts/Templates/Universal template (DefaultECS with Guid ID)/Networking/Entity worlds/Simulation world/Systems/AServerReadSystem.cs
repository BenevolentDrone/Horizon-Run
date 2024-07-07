using System;
using System.Collections.Generic;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Time;

using HereticalSolutions.Messaging;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.LiteNetLib;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

using LiteNetLib;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public abstract class AServerReadSystem<TComponent, TServerComponent, TIDValuePair, TDeltaPacket>
        : ISystem<float>
    {
        protected const float DEFAULT_CONTROL_PACKET_FREQUENCY_CHANCE = 0.05f;
        
        protected readonly NetworkDeltaReplicationSettings networkDeltaReplicationSettings;
        
        protected readonly ITimerManager simulationTimerManager;
        
        protected readonly INetworkHost host;
        
        protected readonly UniversalTemplateEntityManager entityManager;
        
        protected readonly INonAllocMessageSender networkBusAsSender;
        
        protected readonly EntitySet targetEntitiesSet;
        
        protected readonly List<TIDValuePair> dirtyEntities;
        
        protected readonly ILoggerResolver loggerResolver;

        protected readonly ILogger logger;
        
        public bool IsEnabled { get; set; } = true;
        
        protected float controlPacketFrequencyChance;

        protected ushort timerHandle;
        
        protected bool dirty = false;
        
        public AServerReadSystem(
            World world,
            NetworkDeltaReplicationSettings networkDeltaReplicationSettings,
            ITimerManager simulationTimerManager,
            INetworkHost host,
            UniversalTemplateEntityManager entityManager,
            INonAllocMessageSender networkBusAsSender,
            List<TIDValuePair> dirtyEntities,
            ILoggerResolver loggerResolver,
            ILogger logger = null,
            float controlPacketFrequencyChance = DEFAULT_CONTROL_PACKET_FREQUENCY_CHANCE)
        {
            this.networkDeltaReplicationSettings = networkDeltaReplicationSettings;
            
            this.simulationTimerManager = simulationTimerManager;
            
            this.host = host;

            this.entityManager = entityManager;
            
            this.networkBusAsSender = networkBusAsSender;
            
            this.dirtyEntities = dirtyEntities;
            
            this.loggerResolver = loggerResolver;
            
            this.logger = logger;
            
            this.controlPacketFrequencyChance = controlPacketFrequencyChance;
            
            targetEntitiesSet = world
                .GetEntities()
                .With<TComponent>()
                .With<TServerComponent>()
                .AsSet();
            
            //Create a timer
            if (!simulationTimerManager.CreateTimer(
                out timerHandle,
                out var timer))
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"ERROR CREATING TIMER FROM TIME MANAGER {simulationTimerManager.ID}"));
            }
            
            timer.Reset();

            timer.Repeat = true;

            timer.FlushTimeElapsedOnRepeat = true;

            ISubscription timerTickSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                (timerArg) => OnTick(),
                loggerResolver);

            timer.OnFinish.Subscribe(
                (ISubscriptionHandler<
                    INonAllocSubscribableSingleArgGeneric<IRuntimeTimer>,
                    IInvokableSingleArgGeneric<IRuntimeTimer>>)
                timerTickSubscription);
            
            timer.Start(
                networkDeltaReplicationSettings.DeltaTimeout);
        }

        public void Update(
            float deltaTime)
        {
            if (!dirty)
                return;
            
            dirtyEntities.Clear();
            
            foreach (Entity entity in targetEntitiesSet.GetEntities())
            {
                if (!entity.IsAlive)
                    continue;
                
                var targetComponent = entity.Get<TComponent>();

                ref var serverComponent = ref entity.Get<TServerComponent>();

                if (ComponentChanged(
                    targetComponent,
                    serverComponent)
                    || UnityEngine.Random.Range(0f, 1f) < controlPacketFrequencyChance)
                {
                    ReadComponentToServerComponent(
                        targetComponent,
                        ref serverComponent,
                        host.Tick);
                    
                    if (!NetworkEntityHelpers.TryGetNetworkID(
                        entity,
                        entityManager,
                        out var networkID))
                    {
                        continue;
                    }
                    
                    var idValuePair = CreateIDValuePair(
                        networkID,
                        targetComponent);
                    
                    //TODO: check
                    if (idValuePair.Equals(default))
                    {
                        continue;
                    }
                    
                    dirtyEntities.Add(
                        idValuePair);
                }
            }

            int currentIndex = 0;
            
            while (dirtyEntities.Count > currentIndex)
            {
                TIDValuePair[] payload = new TIDValuePair[
                    Mathf.Min(
                        dirtyEntities.Count - currentIndex,
                        GetMaxCapacityPerPacket())];

                for (int i = 0; i < payload.Length; i++)
                {
                    payload[i] = dirtyEntities[currentIndex];

                    currentIndex++;
                }
                
                networkBusAsSender
                    .PopMessage<ServerSendPacketMessage>(
                        out var sendPacketMessage)
                    .Write<ServerSendPacketMessage>(
                        sendPacketMessage,
                        new object[]
                        {
                            byte.MaxValue,
                            typeof(TDeltaPacket),
                            CreateDeltaPacket(
                                host.Tick,
                                (ushort)payload.Length,
                                payload),
                            null,
                            DeliveryMethod.Unreliable
                        })
                    .SendImmediately<ServerSendPacketMessage>(
                        sendPacketMessage);
            }
            
            dirty = false;
        }
        
        protected void OnTick()
        {
            dirty = true;
        }
        
        public void Dispose()
        {
            simulationTimerManager.TryDestroyTimer(timerHandle);
        }

        protected abstract bool ComponentChanged(
            TComponent component,
            TServerComponent serverComponent);
        
        protected abstract void ReadComponentToServerComponent(
            TComponent component,
            ref TServerComponent serverComponent,
            ushort tick);
        
        protected abstract TIDValuePair CreateIDValuePair(
            ushort networkID,
            TComponent component);
        
        protected abstract int GetMaxCapacityPerPacket();

        protected abstract TDeltaPacket CreateDeltaPacket(
            ushort tick,
            ushort count,
            TIDValuePair[] payload);
    }
}