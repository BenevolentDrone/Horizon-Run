namespace HereticalSolutions.Collections.Managed
{
    public enum EBufferElementState
    {
        //Default state
        CONSUMED, // Awaiting a producer to produce

        PRODUCING,          // Being produced by a producer

        PRODUCED,           // Produced and awaiting consumption

        CONSUMING          // Being consumed by a consumer
    }
}