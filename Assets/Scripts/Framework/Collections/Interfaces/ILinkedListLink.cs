namespace HereticalSolutions.Collections
{
    public interface ILinkedListLink<T>
    {
        T Value { get; set; }
        
        ILinkedListLink<T> Previous { get; set; }
        
        ILinkedListLink<T> Next { get; set; }
    }
}