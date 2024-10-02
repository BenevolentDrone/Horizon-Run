namespace HereticalSolutions.SpacePartitioning
{
    public class ValueSpaceData<TValue>
    {
        public TValue Value;

        public Bounds2D Bounds;
        
        public Node<TValue> CurrentNode;
    }
}