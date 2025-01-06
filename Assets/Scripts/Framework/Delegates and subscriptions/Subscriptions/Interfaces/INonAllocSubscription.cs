namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscription
    {
        bool Active { get; }

        void Subscribe(
            INonAllocSubscribable publisher);

        void Unsubscribe();
    }
}