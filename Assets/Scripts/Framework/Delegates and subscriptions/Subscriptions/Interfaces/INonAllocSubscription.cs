namespace HereticalSolutions.Delegates
{
    public interface INonAllocSubscription
    {
        bool Active { get; }

        bool Subscribe(
            INonAllocSubscribable publisher);

        bool Unsubscribe();
    }
}