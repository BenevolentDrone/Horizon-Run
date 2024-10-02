namespace HereticalSolutions.Repositories
{
    public interface IOneToOneMap<TValue1, TValue2>
        : IReadOnlyOneToOneMap<TValue1, TValue2>
    {
        void Add(
            TValue1 leftValue,
            TValue2 rightValue);
        
        bool TryAdd(
            TValue1 leftValue,
            TValue2 rightValue);
        
        void UpdateByLeft(
            TValue1 leftValue,
            TValue2 rightValue);
        
        void UpdateByRight(
            TValue1 leftValue,
            TValue2 rightValue);
        
        bool TryUpdateByLeft(
            TValue1 leftValue,
            TValue2 rightValue);
        
        bool TryUpdateByRight(
            TValue1 leftValue,
            TValue2 rightValue);
        
        void AddOrUpdateByLeft(
            TValue1 leftValue,
            TValue2 rightValue);
        
        void AddOrUpdateByRight(
            TValue1 leftValue,
            TValue2 rightValue);
        
        void RemoveByLeft(TValue1 leftValue);
        
        void RemoveByRight(TValue2 rightValue);
        
        bool TryRemoveByLeft(TValue1 leftValue);
        
        bool TryRemoveByRight(TValue2 rightValue);
        
        void Clear();
    }
}