namespace HereticalSolutions.Hierarchy
{
    public interface IHierarchySubject<TValue>
    {
        IHierarchyNode<TValue> HierarchyNode { get; }
    }
}