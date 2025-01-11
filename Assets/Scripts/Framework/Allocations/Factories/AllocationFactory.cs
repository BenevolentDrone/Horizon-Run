using System;
using System.Threading.Tasks;

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
            Func<TValue> allocationDelegate)
            where TValue : TResult
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
            Type valueType,
            object[] arguments)
        {
            return Activator.CreateInstance(valueType, arguments);
        }

        #region Async

        public static async Task<T> AsyncNullAllocationDelegate<T>()
        {
            return default(T);
        }

        public static async Task<T> AsyncInstanceAllocationDelegate<T>(
            T instance)
        {
            return instance;
        }

        public static async Task<T> AsyncFuncAllocationDelegate<T>(
            Func<T> allocationDelegate)
        {
            return (allocationDelegate != null)
                ? allocationDelegate.Invoke()
                : default(T);
        }

        public static async Task<TResult> AsyncFuncAllocationDelegate<TResult, TValue>(
            Func<TValue> allocationDelegate)
            where TValue : TResult
        {
            TValue result = (allocationDelegate != null)
                ? allocationDelegate.Invoke()
                : default(TValue);

            return (TResult)result;
        }

        public static async Task<T> AsyncActivatorAllocationDelegate<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public static async Task<TResult> AsyncActivatorAllocationDelegate<TResult, TValue>()
        {
            return (TResult)Activator.CreateInstance(typeof(TValue));
        }

        public static async Task<object> AsyncActivatorAllocationDelegate(
            Type valueType)
        {
            return Activator.CreateInstance(valueType);
        }

        public static async Task<T> AsyncActivatorAllocationDelegate<T>(
            object[] arguments)
        {
            return (T)Activator.CreateInstance(typeof(T), arguments);
        }

        public static async Task<TResult> AsyncActivatorAllocationDelegate<TResult, TValue>(
            object[] arguments)
        {
            return (TResult)Activator.CreateInstance(typeof(TValue), arguments);
        }

        public static async Task<object> AsyncActivatorAllocationDelegate(
            Type valueType,
            object[] arguments)
        {
            return Activator.CreateInstance(valueType, arguments);
        }

        #endregion
    }
}