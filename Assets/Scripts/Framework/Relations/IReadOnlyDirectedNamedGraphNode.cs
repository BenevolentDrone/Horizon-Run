using System.Collections.Generic;

namespace HereticalSolutions.Relations
{
    public interface IReadOnlyDirectedNamedGraphNode<TContents>
    {
        TContents Contents { get; }

        bool HasRelation(string key);
        
        IReadOnlyDirectedNamedGraphNode<TContents> GetRelation(string key);
        
        bool TryGetRelation(
            string key,
            out IReadOnlyDirectedNamedGraphNode<TContents> value);
        
        IEnumerable<string> AllRelationKeys { get; }
        
        IEnumerable<IReadOnlyDirectedNamedGraphNode<TContents>> AllRelationValues { get; }
        
        IEnumerable<RelationDTO<TContents>> RelationsReceived { get; }
    }
}