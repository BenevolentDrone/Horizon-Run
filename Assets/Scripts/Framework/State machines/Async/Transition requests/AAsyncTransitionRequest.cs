using System;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Delegates;

namespace HereticalSolutions.StateMachines
{
    public abstract class AAsyncTransitionRequest
    {
        protected readonly object lockObject = new object();

        protected ETransitionState transitionState = ETransitionState.UNINITIALISED;

        public ETransitionState TransitionState
        {
            get
            {
                lock (lockObject)
                {
                    return transitionState;
                }
            }
            set
            {
                lock (lockObject)
                {
                    transitionState = value;
                }
            }
        }

        protected  EAsyncTransitionRules rules = EAsyncTransitionRules.EXIT_THEN_ENTER;

        public EAsyncTransitionRules Rules
        {
            get
            {
                lock (lockObject)
                {
                    return rules;
                }
            }
            set
            {
                lock (lockObject)
                {
                    rules = value;
                }
            }
        }

        protected  bool commencePreviousStateExitStart = true;

        public bool CommencePreviousStateExitStart
        {
            get
            {
                lock (lockObject)
                {
                    return commencePreviousStateExitStart;
                }
            }
            set
            {
                lock (lockObject)
                {
                    commencePreviousStateExitStart = value;
                }
            }
        }

        protected INonAllocSubscribable onPreviousStateExited;

        public INonAllocSubscribable OnPreviousStateExited
        {
            get
            {
                lock (lockObject)
                {
                    return onPreviousStateExited;
                }
            }
            set
            {
                lock (lockObject)
                {
                    onPreviousStateExited = value;
                }
            }
        }

        protected bool commencePreviousStateExitFinish = true;

        public bool CommencePreviousStateExitFinish
        {
            get
            {
                lock (lockObject)
                {
                    return commencePreviousStateExitFinish;
                }
            }
            set
            {
                lock (lockObject)
                {
                    commencePreviousStateExitFinish = value;
                }
            }
        }

        protected bool commenceNextStateEnterStart = true;

        public bool CommenceNextStateEnterStart
        {
            get
            {
                lock (lockObject)
                {
                    return commenceNextStateEnterStart;
                }
            }
            set
            {
                lock (lockObject)
                {
                    commenceNextStateEnterStart = value;
                }
            }
        }

        protected INonAllocSubscribable onNextStateEntered;

        public INonAllocSubscribable OnNextStateEntered
        {
            get
            {
                lock (lockObject)
                {
                    return onNextStateEntered;
                }
            }
            set
            {
                lock (lockObject)
                {
                    onNextStateEntered = value;
                }
            }
        }

        protected bool commenceNextStateEnterFinish = true;

        public bool CommenceNextStateEnterFinish
        {
            get
            {
                lock (lockObject)
                {
                    return commenceNextStateEnterFinish;
                }
            }
            set
            {
                lock (lockObject)
                {
                    commenceNextStateEnterFinish = value;
                }
            }
        }

        protected IProgress<float> previousStateExitProgress;

        public IProgress<float> PreviousStateExitProgress
        {
            get
            {
                lock (lockObject)
                {
                    return previousStateExitProgress;
                }
            }
            set
            {
                lock (lockObject)
                {
                    previousStateExitProgress = value;
                }
            }
        }

        protected IProgress<float> nextStateEnterProgress;

        public IProgress<float> NextStateEnterProgress
        {
            get
            {
                lock (lockObject)
                {
                    return nextStateEnterProgress;
                }
            }
            set
            {
                lock (lockObject)
                {
                    nextStateEnterProgress = value;
                }
            }
        }

        protected AsyncExecutionContext asyncContext;

        public AsyncExecutionContext AyncContext
        {
            get
            {
                lock (lockObject)
                {
                    return asyncContext;
                }
            }
            set
            {
                lock (lockObject)
                {
                    asyncContext = value;
                }
            }
        }
    }
}