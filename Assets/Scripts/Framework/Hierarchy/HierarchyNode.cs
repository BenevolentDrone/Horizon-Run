using System.Collections.Generic;
using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Hierarchy
{
    public class HierarchyNode<TContents>
        : IHierarchyNode<TContents>,
          ICleanuppable
    {
        private readonly List<IReadOnlyHierarchyNode<TContents>> children;

        private IReadOnlyHierarchyNode<TContents> parent;
        
        private TContents contents;
        
        public HierarchyNode(
            List<IReadOnlyHierarchyNode<TContents>> children)
        {
            this.children = children;
            
            parent = null;
            
            contents = default;
        }
        
        #region IHierarchyNode

        #region IReadOnlyHierarchyNode

        public TContents Contents
        {
            get => contents;
            set => contents = value;
        }

        public bool IsRoot { get => parent == null; }

        public bool IsLeaf { get => children.Count == 0; }

        public IReadOnlyHierarchyNode<TContents> Parent
        {
            get => parent;
        }

        public IEnumerable<IReadOnlyHierarchyNode<TContents>> Children
        {
            get => children;
        }
        
        public int ChildCount { get => children.Count; }

        #endregion

        public void SetParent(
            IReadOnlyHierarchyNode<TContents> parent,
            bool addToParentsChildren = true)
        {
            if (this.parent != null)
            {
                ((IHierarchyNode<TContents>)this.parent)?.RemoveChild(
                    this,
                    false);
            }

            this.parent = parent;

            if (addToParentsChildren)
                ((IHierarchyNode<TContents>)parent)?.AddChild(
                    this,
                    false);
        }

        public void RemoveParent(
            bool removeFromParentsChildren = true)
        {
            if (removeFromParentsChildren)
                ((IHierarchyNode<TContents>)parent)?.RemoveChild(
                    this,
                    false);

            parent = null;
        }

        public void AddChild(
            IReadOnlyHierarchyNode<TContents> child,
            bool setAsChildsParent = true)
        {
            if (children.Contains(child))
                return;
            
            children.Add(child);

            if (setAsChildsParent)
                ((IHierarchyNode<TContents>)child)?.SetParent(
                    this,
                    false);
        }

        public void RemoveChild(
            IReadOnlyHierarchyNode<TContents> child,
            bool removeFromChildsParent = true)
        {
            if (!children.Contains(child))
                return;

            if (removeFromChildsParent)
                ((IHierarchyNode<TContents>)child)?.RemoveParent(
                    false);

            children.Remove(child);
        }

        public void RemoveAllChildren(
            bool removeFromChildrensParent = true)
        {
            if (removeFromChildrensParent)
                foreach (var child in children)
                    ((IHierarchyNode<TContents>)child)?.RemoveParent(false);

            children.Clear();
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            RemoveAllChildren();
            
            RemoveParent();
            
            parent = null;
            
            contents = default;
            
            children.Clear();
        }

        #endregion
    }
}