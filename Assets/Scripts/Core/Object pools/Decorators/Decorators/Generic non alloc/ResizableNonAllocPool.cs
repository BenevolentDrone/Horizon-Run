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
    public class ResizableNonAllocPool<T>
        : INonAllocDecoratedPool<T>,
          IResizable<IPoolElement<T>>,
          IModifiable<INonAllocPool<T>>,
          ITopUppable<IPoolElement<T>>,
          ICountUpdateable,
          ICleanUppable,
          IDisposable
    {
        private readonly ICountUpdateable contentsAsCountUpdateable;

        private readonly IPushBehaviourHandler<T> pushBehaviourHandler;


        private readonly Func<T> topUpAllocationDelegate;

        private readonly bool topUpIfElementValueIsNull;


        private readonly ILogger logger;


        private INonAllocPool<T> contents;


        public ResizableNonAllocPool(
            INonAllocPool<T> contents,
            ICountUpdateable contentsAsCountUpdateable,
            
            Action<ResizableNonAllocPool<T>> resizeDelegate,
            AllocationCommand<IPoolElement<T>> resizeAllocationCommand,

            Func<T> topUpAllocationDelegate,
            bool topUpIfElementValueIsNull,

            ILogger logger = null)
        {
            this.contents = contents;

            this.contentsAsCountUpdateable = contentsAsCountUpdateable;


            this.resizeDelegate = resizeDelegate;


            this.topUpAllocationDelegate = topUpAllocationDelegate;

            this.topUpIfElementValueIsNull = topUpIfElementValueIsNull;


            ResizeAllocationCommand = resizeAllocationCommand;

            pushBehaviourHandler = new PushToDecoratedPoolBehaviour<T>(this);

            this.logger = logger;
        }

        #region IModifiable

        public INonAllocPool<T> Contents { get => contents; }

        public void UpdateContents(INonAllocPool<T> newContents)
        {
            contents = newContents;
        }

        public void UpdateCount(int newCount)
        {
            logger?.Log<ResizableNonAllocPool<T>>(
                $"NEW POOL SIZE: {newCount}");

            contentsAsCountUpdateable.UpdateCount(newCount);
        }

        #endregion

        #region IResizable

        public AllocationCommand<IPoolElement<T>> ResizeAllocationCommand { get; private set; }

        protected Action<ResizableNonAllocPool<T>> resizeDelegate;

        public void Resize()
        {
            logger?.Log<ResizableNonAllocPool<T>>(
                "RESIZE INVOKED");

            resizeDelegate(this);
        }

        #endregion

        #region ITopUppable

        public void TopUp(IPoolElement<T> element)
        {
            logger?.Log<ResizableNonAllocPool<T>>(
                "TOP UP INVOKED");

            element.Value = topUpAllocationDelegate.Invoke();
        }

        #endregion

        #region INonAllocDecoratedPool

        public IPoolElement<T> Pop(IPoolDecoratorArgument[] args = null)
        {
            #region Resize

            if (!contents.HasFreeSpace)
            {
                logger?.Log<ResizableNonAllocPool<T>>(
                    "POOL SIZE EXCEEDED, RESIZING");

                resizeDelegate(this);
            }

            #endregion

            IPoolElement<T> result = contents.Pop();

            #region Top up

            if (topUpIfElementValueIsNull
                && EqualityComparer<T>.Default.Equals(result.Value, default(T)))
            {
                logger?.Log<ResizableNonAllocPool<T>>(
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

        public void Push(
            IPoolElement<T> instance,
            bool decoratorsOnly = false)
        {
            if (!decoratorsOnly)
                contents.Push(instance);
        }

        //public bool HasFreeSpace { get { return contents.HasFreeSpace; } }
        public bool HasFreeSpace { get { return true; } } // ¯\_(ツ)_/¯

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (contents is ICleanUppable)
                (contents as ICleanUppable).Cleanup();

            if (pushBehaviourHandler is ICleanUppable)
                (pushBehaviourHandler as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (contents is IDisposable)
                (contents as IDisposable).Dispose();

            if (pushBehaviourHandler is IDisposable)
                (pushBehaviourHandler as IDisposable).Dispose();
        }

        #endregion
    }
}