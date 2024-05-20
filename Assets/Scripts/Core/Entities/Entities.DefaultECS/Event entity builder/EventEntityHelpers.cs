using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public static class EventEntityHelpers
    {
        public static bool TryGetEventSourceEntityID<TEntityID>(
            Entity eventEntity,
            out TEntityID eventSourceEntityID,
            bool checkEventComponent = true)
        {
            eventSourceEntityID = default;
            
            if (checkEventComponent
                && !eventEntity.Has<EventSourceEntityComponent<TEntityID>>())
                return false;

            var eventSourceComponent = eventEntity.Get<EventSourceEntityComponent<TEntityID>>();

            eventSourceEntityID = eventSourceComponent.SourceID;

            return true;
        }
        
        public static bool TryGetEventSourceWorldLocalEntity(
            Entity eventEntity,
            out Entity eventSourceEntity,
            bool checkEventComponent = true)
        {
            eventSourceEntity = default;
            
            if (checkEventComponent
                && !eventEntity.Has<EventSourceWorldLocalEntityComponent<Entity>>())
                return false;

            var eventSourceComponent = eventEntity.Get<EventSourceWorldLocalEntityComponent<Entity>>();

            eventSourceEntity = eventSourceComponent.Source;

            if (!eventSourceEntity.IsAlive)
                return false;

            return true;
        }
        
        public static bool TryGetEventReceiverEntityID<TEntityID>(
            Entity eventEntity,
            out TEntityID eventReceiverEntityID,
            bool checkEventComponent = true)
        {
            eventReceiverEntityID = default;
            
            if (checkEventComponent
                && !eventEntity.Has<EventReceiverEntityComponent<TEntityID>>())
                return false;

            var eventReceiverComponent = eventEntity.Get<EventReceiverEntityComponent<TEntityID>>();

            eventReceiverEntityID = eventReceiverComponent.ReceiverID;

            return true;
        }
        
        public static bool TryGetEventReceiverWorldLocalEntity(
            Entity eventEntity,
            out Entity eventReceiverEntity,
            bool checkEventComponent = true)
        {
            eventReceiverEntity = default;
            
            if (checkEventComponent
                && !eventEntity.Has<EventReceiverWorldLocalEntityComponent<Entity>>())
                return false;

            var eventReceiverComponent = eventEntity.Get<EventReceiverWorldLocalEntityComponent<Entity>>();

            eventReceiverEntity = eventReceiverComponent.Receiver;

            if (!eventReceiverEntity.IsAlive)
                return false;

            return true;
        }
        
        public static bool TryGetEventTargetEntityID<TEntityID>(
            Entity eventEntity,
            out TEntityID eventReceiverEntityID,
            bool checkEventComponent = true)
        {
            eventReceiverEntityID = default;
            
            if (checkEventComponent
                && !eventEntity.Has<EventTargetEntityComponent<TEntityID>>())
                return false;

            var eventTargetComponent = eventEntity.Get<EventTargetEntityComponent<TEntityID>>();

            eventReceiverEntityID = eventTargetComponent.TargetID;

            return true;
        }
        
        public static bool TryGetEventTargetWorldLocalEntity(
            Entity eventEntity,
            out Entity eventTargetEntity,
            bool checkEventComponent = true)
        {
            eventTargetEntity = default;
            
            if (checkEventComponent
                && !eventEntity.Has<EventTargetWorldLocalEntityComponent<Entity>>())
                return false;

            var eventTargetComponent = eventEntity.Get<EventTargetWorldLocalEntityComponent<Entity>>();

            eventTargetEntity = eventTargetComponent.Target;

            if (!eventTargetEntity.IsAlive)
                return false;

            return true;
        }
    }
}