using System;
using System.Collections.Generic;

using HereticalSolutions.Collections;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.Behaviours;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
    public class SupplyAndMergePool<T> :
        INonAllocDecoratedPool<T>,
        IAppendable<IPoolElement<T>>,
        ITopUppable<IPoolElement<T>>,
        ICleanUppable,
        IDisposable
    {
        private readonly INonAllocPool<T> basePool;
        
        private readonly INonAllocPool<T> supplyPool;
        

        private readonly IIndexable<IPoolElement<T>> supplyPoolAsIndexable;
        
        private readonly IFixedSizeCollection<IPoolElement<T>> supplyPoolAsFixedSizeCollection;
        

        private readonly IPushBehaviourHandler<T> pushBehaviourHandler;


        private readonly Action<INonAllocPool<T>, INonAllocPool<T>, AllocationCommand<IPoolElement<T>>> mergeDelegate;


        private readonly Func<T> topUpAllocationDelegate;

        private readonly bool topUpIfElementValueIsNull;


        private readonly ILogger logger;

        public SupplyAndMergePool(
            INonAllocPool<T> basePool,
            INonAllocPool<T> supplyPool,

            IIndexable<IPoolElement<T>> supplyPoolAsIndexable,
            IFixedSizeCollection<IPoolElement<T>> supplyPoolAsFixedSizeCollection,

            AllocationCommand<IPoolElement<T>> appendAllocationCommand,
            Action<INonAllocPool<T>, INonAllocPool<T>, AllocationCommand<IPoolElement<T>>> mergeDelegate,
            
            Func<T> topUpAllocationDelegate,
            bool topUpIfElementValueIsNull,

            ILogger logger = null)
        {
            this.basePool = basePool;
            
            this.supplyPool = supplyPool;
            

            this.supplyPoolAsIndexable = supplyPoolAsIndexable;
            
            this.supplyPoolAsFixedSizeCollection = supplyPoolAsFixedSizeCollection;
            

            this.mergeDelegate = mergeDelegate;
            

            this.topUpAllocationDelegate = topUpAllocationDelegate;

            this.topUpIfElementValueIsNull = topUpIfElementValueIsNull;


            this.logger = logger;
            

            AppendAllocationCommand = appendAllocationCommand;
            
            pushBehaviourHandler = new PushToDecoratedPoolBehaviour<T>(this);
        }

        #region IAppendable

        public AllocationCommand<IPoolElement<T>> AppendAllocationCommand { get; private set; }

        public IPoolElement<T> Append()
        {
            logger?.Log<SupplyAndMergePool<T>>(
                "APPEND INVOKED");

            if (!supplyPool.HasFreeSpace)
            {
                logger?.Log<SupplyAndMergePool<T>>(
                    "POOL SIZE EXCEEDED, MERGING SUPPLY POOL INTO BASE POOL");

                MergeSupplyIntoBase();
            }

            IPoolElement<T> result = supplyPool.Pop();

            return result;
        }

        #endregion

        #region ITopUppable

        public void TopUp(IPoolElement<T> element)
        {
            logger?.Log<SupplyAndMergePool<T>>(
                "TOP UP INVOKED");

            element.Value = topUpAllocationDelegate.Invoke();
        }

        #endregion

        #region Merge

        private void MergeSupplyIntoBase()
        {
            logger?.Log<SupplyAndMergePool<T>>(
                $"MERGE SUPPLY INTO BASE INVOKED. SUPPLY POOL: [{supplyPoolAsIndexable.Count} / {supplyPoolAsFixedSizeCollection.Capacity}]");

            mergeDelegate.Invoke(
                basePool,
                supplyPool,
                AppendAllocationCommand);
        }

        private void TopUpAndMerge()
        {
            logger?.Log<SupplyAndMergePool<T>>(
                $"TOP UP AND MERGE INVOKED. SUPPLY POOL: [{supplyPoolAsIndexable.Count} / {supplyPoolAsFixedSizeCollection.Capacity}]");

            for (int i = supplyPoolAsIndexable.Count; i < supplyPoolAsFixedSizeCollection.Capacity; i++)
                TopUp(supplyPoolAsFixedSizeCollection.ElementAt(i));

            MergeSupplyIntoBase();
        }

        #endregion

        #region INonAllocDecoratedPool
        
        public IPoolElement<T> Pop(IPoolDecoratorArgument[] args = null)
        {
            #region Append from argument
            
            if (args.TryGetArgument<AppendArgument>(out var arg))
            {
                logger?.Log<SupplyAndMergePool<T>>(
                    "APPEND ARGUMENT RECEIVED, APPENDING");

                var appendee = Append();

                #region Update push behaviour

                var appendeeElementAsPushable = (IPushable<T>)appendee; 
            
                appendeeElementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);

                #endregion
                
                return appendee;
            }
            
            #endregion
            
            #region Top up and merge
            
            if (!basePool.HasFreeSpace)
            {
                logger?.Log<SupplyAndMergePool<T>>(
                    "POOL SIZE EXCEEDED, TOPPING UP SUPPLY POOL AND MERGING INTO BASE POOL");

                TopUpAndMerge();
            }
            
            #endregion

            IPoolElement<T> result = basePool.Pop();

            #region Top up

            if (topUpIfElementValueIsNull
                && EqualityComparer<T>.Default.Equals(result.Value, default(T)))
            {
                logger?.Log<SupplyAndMergePool<T>>(
                    "POOL ELEMENT IS NULL OR DEFAULT, TOPPING UP");

                TopUp(result);
            }
            
            #endregion
            
            #region Update push behaviour
            
            var elementAsPushable = (IPushable<T>)result; 
            
            elementAsPushable.UpdatePushBehaviour(pushBehaviourHandler);
            
            #endregion

            return result;
        }

        public void Push(IPoolElement<T> instance, bool decoratorsOnly = false)
        {
            if (decoratorsOnly)
                return;

            #region Top up and merge
            
            var instanceIndex = instance.Metadata.Get<IIndexed>().Index;

            if (instanceIndex > -1
                && instanceIndex < supplyPoolAsIndexable.Count
                && supplyPoolAsIndexable[instanceIndex] == instance)
            {
                //TODO: check out reasoning
                logger?.Log<SupplyAndMergePool<T>>(
                    "PUSHED ELEMENT'S INDEX IS NOT (-1), TOPPING UP SUPPLY POOL AND MERGING INTO BASE POOL");

                TopUpAndMerge();
            }
            
            #endregion

            basePool.Push(instance);
        }
        
        public bool HasFreeSpace { get { return true; } }  // ¯\_(ツ)_/¯

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (basePool is ICleanUppable)
                (basePool as ICleanUppable).Cleanup();

            if (supplyPool is ICleanUppable)
                (supplyPool as ICleanUppable).Cleanup();

            if (pushBehaviourHandler is ICleanUppable)
                (pushBehaviourHandler as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (basePool is IDisposable)
                (basePool as IDisposable).Dispose();

            if (supplyPool is ICleanUppable)
                (supplyPool as ICleanUppable).Cleanup();

            if (pushBehaviourHandler is IDisposable)
                (pushBehaviourHandler as IDisposable).Dispose();
        }

        #endregion
    }
}