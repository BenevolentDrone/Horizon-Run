namespace HereticalSolutions.Entities
{
    [NetworkEventComponent]
    public struct EventTargetEntityComponent<TEntityID>
    {
        public TEntityID TargetID;
    }
}