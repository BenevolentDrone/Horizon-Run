namespace HereticalSolutions.SpacePartitioning
{
    public interface IQuadTree<TValue>
    {
        Node<TValue> AllocateNode(
            Bounds2D bounds,
            Node<TValue> parent,
            int depth);

        void DisposeNode(Node<TValue> node);

        ValueSpaceData<TValue> AllocateValueSpaceData(
            TValue value,
            Bounds2D bounds,
            Node<TValue> node);

        void DisposeValueSpaceData(ValueSpaceData<TValue> valueData);
    }
}