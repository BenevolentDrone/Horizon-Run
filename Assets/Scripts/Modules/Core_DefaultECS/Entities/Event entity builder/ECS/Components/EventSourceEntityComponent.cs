namespace HereticalSolutions.Modules.Core_DefaultECS
{
    [NetworkEventComponent]
    public struct EventSourceEntityComponent<TEntityID>
    {
        public TEntityID SourceID;
    }
}