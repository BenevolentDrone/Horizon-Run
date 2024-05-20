using System;
using System.Threading.Tasks;

using UnityEngine;

namespace HereticalSolutions.StateMachines
{
    public class AnimationHandler
    {
        private bool inProgress = false;
        
        private Action<IState, Action> playAnimationDelegate;
        
        public AnimationHandler(
            Action<IState, Action> playAnimationDelegate)
        {
            this.playAnimationDelegate = playAnimationDelegate;
        }

        public async Task Play(IState state)
        {
            if (inProgress)
            {
                Debug.LogError("[AnimationHandler] ANIMATION IS IN PROGRESS");
                
                return;
            }

            inProgress = true;

            playAnimationDelegate?.Invoke(state, OnComplete);

            while (inProgress)
            {
                await Task.Yield();
            }
        }

        private void OnComplete()
        {
            if (!inProgress)
            {
                Debug.LogError("[AnimationHandler] ANIMATION IS NOT IN PROGRESS");
                
                return;
            }

            inProgress = false;
        }
    }
}