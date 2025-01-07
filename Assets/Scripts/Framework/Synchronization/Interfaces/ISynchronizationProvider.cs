using HereticalSolutions.Delegates;

namespace HereticalSolutions.Synchronization
{
    public interface ISynchronizationProvider
    {
        void Subscribe(INonAllocSubscription subscription);

        void Unsubscribe(INonAllocSubscription subscription);

        void UnsubscribeAll();
    }
}