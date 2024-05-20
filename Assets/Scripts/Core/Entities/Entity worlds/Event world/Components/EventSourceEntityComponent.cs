namespace HereticalSolutions.Entities
{
    [NetworkEventComponent]
    public struct EventSourceEntityComponent<TEntityID>
    {
        public TEntityID SourceID;
    }
}