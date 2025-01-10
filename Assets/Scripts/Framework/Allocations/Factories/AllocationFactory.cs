using System;

namespace HereticalSolutions.Allocations.Factories
{
    public static class AllocationFactory
    {
        public static T NullAllocationDelegate<T>()
        {
            return default(T);
        }

        public static T InstanceAllocationDelegate<T>(
            T instance)
        {
            return instance;
        }

        public static T FuncAllocationDelegate<T>(
            Func<T> allocationDelegate)
        {
            return (allocationDelegate != null)
                ? allocationDelegate.Invoke()
                : default(T);
        }
		
        public static TResult FuncAllocationDelegate<TResult, TValue>(
            Func<TValue> allocationDelegate) where TValue : TResult
        {
            TValue result = (allocationDelegate != null)
                ? allocationDelegate.Invoke()
                : default(TValue);

            return (TResult)result;
        }

        public static T ActivatorAllocationDelegate<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
		
        public static TResult ActivatorAllocationDelegate<TResult, TValue>()
        {
            return (TResult)Activator.CreateInstance(typeof(TValue));
        }

        public static object ActivatorAllocationDelegate(
            Type valueType)
        {
            return Activator.CreateInstance(valueType);
        }

        public static T ActivatorAllocationDelegate<T>(
            object[] arguments)
        {
            return (T)Activator.CreateInstance(typeof(T), arguments);
        }
        
        public static TResult ActivatorAllocationDelegate<TResult, TValue>(
            object[] arguments)
        {
            return (TResult)Activator.CreateInstance(typeof(TValue), arguments);
        }
        
        public static object ActivatorAllocationDelegate(
            Type valueType, object[] arguments)
        {
            return Activator.CreateInstance(valueType, arguments);
        }
    }
}