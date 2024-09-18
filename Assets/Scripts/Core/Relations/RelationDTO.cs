namespace HereticalSolutions.Relations
{
    public class RelationDTO<TContents>
    {
        public IReadOnlyDirectedNamedGraphNode<TContents> Source;
        
        public string Key;
    }
}