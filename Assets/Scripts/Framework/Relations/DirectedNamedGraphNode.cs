using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Relations
{
    public class DirectedNamedGraphNode<TContents>
        : IDirectedNamedGraphNode<TContents>,
          ICleanuppable
    {
        private readonly IRepository<string, IReadOnlyDirectedNamedGraphNode<TContents>> relations;
        
        private readonly List<RelationDTO<TContents>> relationsReceived;
        
        private TContents contents;
        
        public DirectedNamedGraphNode(
            IRepository<string, IReadOnlyDirectedNamedGraphNode<TContents>> relations,
            List<RelationDTO<TContents>> relationsReceived)
        {
            this.relations = relations;
            
            this.relationsReceived = relationsReceived;
            
            contents = default;
        }
        
        #region IDirectedNamedGraphNode

        #region IReadOnlyDirectedNamedGraphNode

        public TContents Contents
        {
            get => contents;
            set => contents = value;
        }

        public bool HasRelation(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;
            
            return relations.Has(key);
        }

        public IReadOnlyDirectedNamedGraphNode<TContents> GetRelation(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (!relations.Has(key))
            {
                return null;
            }
            
            return relations.Get(key);
        }

        public bool TryGetRelation(
            string key,
            out IReadOnlyDirectedNamedGraphNode<TContents> value)
        {
            if (string.IsNullOrEmpty(key))
            {
                value = null;
                
                return false;
            }

            return relations.TryGet(
                key,
                out value);
        }

        public IEnumerable<string> AllRelationKeys { get => relations.Keys; }
        
        public IEnumerable<IReadOnlyDirectedNamedGraphNode<TContents>> AllRelationValues { get => relations.Values; }

        public IEnumerable<RelationDTO<TContents>> RelationsReceived { get => relationsReceived; }
        
        #endregion

        public void AddRelation(
            string key,
            IReadOnlyDirectedNamedGraphNode<TContents> value,
            bool relativeReceivesRelation = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (relations.Has(key))
            {
                return;
            }

            relations.Add(
                key,
                value);

            if (relativeReceivesRelation)
            {
                ((DirectedNamedGraphNode<TContents>)value)?.ReceiveRelation(
                    this,
                    key);
            }
        }

        public bool TryAddRelation(
            string key,
            IReadOnlyDirectedNamedGraphNode<TContents> value,
            bool relativeReceivesRelation = true)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            bool result = relations.TryAdd(
                key,
                value);

            if (result
                && relativeReceivesRelation)
            {
                ((DirectedNamedGraphNode<TContents>)value)?.ReceiveRelation(
                    this,
                    key);
            }

            return result;
        }

        public void AddOrReplaceRelation(
            string key,
            IReadOnlyDirectedNamedGraphNode<TContents> value,
            bool relativeReceivesRelation = true)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (relativeReceivesRelation
                && relations.Has(key))
            {
                var previousValue = relations.Get(key);
                
                ((DirectedNamedGraphNode<TContents>)previousValue)?.AbandonRelation(
                    this,
                    key);
            }
            
            relations.AddOrUpdate(
                key,
                value);

            if (relativeReceivesRelation)
            {
                ((DirectedNamedGraphNode<TContents>)value)?.ReceiveRelation(
                    this,
                    key);
            }
        }

        public void RemoveRelation(
            string key,
            bool relativeAbandonsRelation = true)
        {
            if (string.IsNullOrEmpty(key))
                return;
            
            if (!relations.Has(key))
                return;
            
            var value = relations.Get(key);
            
            relations.Remove(key);

            if (relativeAbandonsRelation)
            {
                ((DirectedNamedGraphNode<TContents>)value)?.AbandonRelation(
                    this,
                    key);
            }
        }

        public bool TryRemoveRelation(
            string key,
            bool relativeAbandonsRelation = true)
        {
            if (string.IsNullOrEmpty(key))
                return false;
            
            if (!relations.Has(key))
                return false;
            
            var value = relations.Get(key);
            
            relations.TryRemove(key);

            if (relativeAbandonsRelation)
            {
                ((DirectedNamedGraphNode<TContents>)value)?.AbandonRelation(
                    this,
                    key);
            }

            return true;
        }

        public void ReceiveRelation(
            IReadOnlyDirectedNamedGraphNode<TContents> source,
            string key)
        {
            relationsReceived.Add(
                new RelationDTO<TContents>()
                    {
                        Source = source,
                        Key = key
                    });
        }

        public void AbandonRelation(
            IReadOnlyDirectedNamedGraphNode<TContents> source,
            string key)
        {
            relationsReceived.RemoveAll(
                (relation) => relation.Source == source && relation.Key == key);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            foreach (var relationKey in relations.Keys)
            {
                var relationValue = relations.Get(relationKey);
                
                ((DirectedNamedGraphNode<TContents>)relationValue)?.AbandonRelation(
                    this,
                    relationKey);
            }

            foreach (var relation in relationsReceived)
            {
                ((DirectedNamedGraphNode<TContents>)relation.Source)?.RemoveRelation(
                    relation.Key,
                    false);
            }
            
            contents = default;
            
            relations.Clear();
            
            relationsReceived.Clear();
        }

        #endregion
    }
}