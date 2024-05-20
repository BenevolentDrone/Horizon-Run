using System;
using System.Collections.Generic;

namespace HereticalSolutions.Repositories
{
    public interface IReadOnlyOneToOneMap<TValue1, TValue2>
    {
        bool HasLeft(TValue1 key);
        
        bool HasRight(TValue2 key);

        TValue2 GetRight(TValue1 key);
        
        TValue1 GetLeft(TValue2 key);
        
        bool TryGetRight(
            TValue1 key,
            out TValue2 value);
        
        bool TryGetLeft(
            TValue2 key,
            out TValue1 value);
        
        int Count { get; }
        
        IEnumerable<Tuple<TValue1, TValue2>> Values { get; }
        
        IEnumerable<TValue1> LeftValues { get; }
        
        IEnumerable<TValue2> RightValues { get; }
    }
}