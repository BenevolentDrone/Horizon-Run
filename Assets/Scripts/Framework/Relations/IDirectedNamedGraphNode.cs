namespace HereticalSolutions.Relations
{
    public interface IDirectedNamedGraphNode<TContents>
        : IReadOnlyDirectedNamedGraphNode<TContents>
    {
        TContents Contents { set; } //TAKE NOTE THAT THIS IS A SETTER. A GETTER IS IN A READ ONLY INTERFACE
        
        void AddRelation(
            string key,
            IReadOnlyDirectedNamedGraphNode<TContents> value,
            bool relativeReceivesRelation = true);
        
        bool TryAddRelation(
            string key,
            IReadOnlyDirectedNamedGraphNode<TContents> value,
            bool relativeReceivesRelation = true);
        
        void AddOrReplaceRelation(
            string key,
            IReadOnlyDirectedNamedGraphNode<TContents> value,
            bool relativeReceivesRelation = true);
        
        void RemoveRelation(
            string key,
            bool relativeAbandonsRelation = true);
        
        bool TryRemoveRelation(
            string key,
            bool relativeAbandonsRelation = true);
        
        void ReceiveRelation(
            IReadOnlyDirectedNamedGraphNode<TContents> source,
            string key);

        void AbandonRelation(
            IReadOnlyDirectedNamedGraphNode<TContents> source,
            string key);
    }
}