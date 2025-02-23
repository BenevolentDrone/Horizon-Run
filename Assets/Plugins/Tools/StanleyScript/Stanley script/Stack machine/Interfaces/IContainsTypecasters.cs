using System;

namespace HereticalSolutions.StanleyScript
{
    public interface IContainsTypecasters
    {
        bool HasTypecaster(
            ITypecaster typecaster);
        
        bool TryRegisterTypecaster(
            ITypecaster typecaster);
        
        bool TryRemoveTypecaster(
            ITypecaster typecaster);
        
        bool CanCast<TSource, TValue>();
        
        bool CanCast(
            Type sourceType,
            Type targetType);
        
        bool TryCast<TSource, TValue>(
            TSource source,
            out TValue result);

        bool TryCast(
            object source,
            Type targetType,
            out object result);
    }
}