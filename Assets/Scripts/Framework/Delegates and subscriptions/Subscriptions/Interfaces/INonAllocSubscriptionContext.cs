using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscriptionContext<TInvokable>
    {
        TInvokable Delegate { get; }

        IPoolElementFacade<ISubscription> PoolElement { get; }

        bool ValidateActivation(
            INonAllocSubscribable publisher);

        void Activate(
            INonAllocSubscribable publisher,
            IPoolElementFacade<ISubscription> poolElement);

        bool ValidateTermination(
            INonAllocSubscribable publisher);

        void Terminate();
    }
}