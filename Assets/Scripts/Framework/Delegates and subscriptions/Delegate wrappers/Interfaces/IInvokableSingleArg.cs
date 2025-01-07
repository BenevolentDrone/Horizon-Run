using System;

namespace HereticalSolutions.Delegates
{
    public interface IInvokableSingleArg
    {
        Type ValueType { get; }

        void Invoke<TArgument>(
            TArgument value);

        void Invoke(
            Type valueType,
            object value);
    }
}