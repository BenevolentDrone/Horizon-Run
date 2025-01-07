using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscriptionContext<TInvokable>
    {
        TInvokable Delegate { get; }

        //IPoolElementFacade<INonAllocSubscription> PoolElement { get; }

        bool ValidateActivation(
            INonAllocSubscribable publisher);

        void Activate(
            INonAllocSubscribable publisher);
            //IPoolElementFacade<INonAllocSubscription> poolElement);

        bool ValidateTermination(
            INonAllocSubscribable publisher);

        void Terminate();
    }
}