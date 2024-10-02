namespace HereticalSolutions.Modules.Core_DefaultECS
{
    [NetworkEventComponent]
    public struct EventTargetEntityComponent<TEntityID>
    {
        public TEntityID TargetID;
    }
}