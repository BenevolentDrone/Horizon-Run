using System;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StateMachines
{
    public class AsyncTransitionRequest<TBaseState>
        where TBaseState : IState
    {
        protected readonly object lockObject = new object();

        private ETransitionState transitionState = ETransitionState.UNINITIALISED;
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

        private EAsyncTransitionRules rules = EAsyncTransitionRules.EXIT_THEN_ENTER;
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

        private bool commencePreviousStateExitStart = true;
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

        private Action<TBaseState> onPreviousStateExited;
        public Action<TBaseState> OnPreviousStateExited
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

        private bool commencePreviousStateExitFinish = true;
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

        private bool commenceNextStateEnterStart = true;
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

        private Action<TBaseState> onNextStateEntered;
        public Action<TBaseState> OnNextStateEntered
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

        private bool commenceNextStateEnterFinish = true;
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

        private IProgress<float> previousStateExitProgress;
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

        private IProgress<float> nextStateEnterProgress;
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

        private AsyncExecutionContext asyncContext;
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

/*
using System;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StateMachines
{
    public class AsyncTransitionRequest<TBaseState>
        where TBaseState : IState
    {
        public ETransitionState TransitionState = ETransitionState.UNINITIALISED;

        public ITransitionEvent<TBaseState> Event;

        public EAsyncTransitionRules Rules = EAsyncTransitionRules.EXIT_THEN_ENTER;


        public bool CommencePreviousStateExitStart = true;

        public Action<TBaseState> OnPreviousStateExited { get; set; }

        public bool CommencePreviousStateExitFinish = true;


        public bool CommenceNextStateEnterStart = true;

        public Action<TBaseState> OnNextStateEntered { get; set; }

        public bool CommenceNextStateEnterFinish = true;


        public IProgress<float> PreviousStateExitProgress;

        public IProgress<float> NextStateEnterProgress;


        public AsyncExecutionContext AyncContext;
    }
}
*/