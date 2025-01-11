namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscriptionContext<TInvokable>
    {
        TInvokable Delegate { get; }

        bool ValidateActivation(
            INonAllocSubscribable publisher);

        void Activate(
            INonAllocSubscribable publisher);

        bool ValidateTermination(
            INonAllocSubscribable publisher);

        void Terminate();
    }
}