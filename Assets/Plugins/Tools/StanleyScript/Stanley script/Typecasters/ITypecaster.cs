using System;

namespace HereticalSolutions.StanleyScript
{
    public interface ITypecaster
    {
        Type TargetType { get; }
        
        Type[] SupportedSourceTypes { get; }
        
        bool WillCast<TSource>();
        
        bool WillCast(
            Type sourceType);
        
        bool TryCast<TSource, TValue>(
            TSource source,
            out TValue result);
        
        bool TryCast(
            object source,
            out object result);
    }
}