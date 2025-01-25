using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace HereticalSolutions.StateMachines
{
    public class SceneTransitionController
        : IAsyncTransitionController<SceneState>
    {
        public async Task EnterState(
            SceneState state,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            asyncContext.Progress?.Report(0f);
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
                state.SceneID,
                LoadSceneMode.Additive);
            
            while (!asyncLoad.isDone)
            {
                asyncContext.Progress?.Report(asyncLoad.progress);
                
                await Task.Yield();
            }

            asyncContext.Progress?.Report(1f);
            
            if (state.SetAsActiveOnLoad)
                SceneManager.SetActiveScene(
                    SceneManager.GetSceneByName(state.SceneID));
        }

        public async Task ExitState(
            SceneState state,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            asyncContext.Progress?.Report(0f);
            
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(state.SceneID);
            
            while (!asyncUnload.isDone)
            {
                asyncContext.Progress?.Report(asyncUnload.progress);
                
                await Task.Yield();
            }
            
            asyncContext.Progress?.Report(1f);
        }
    }
}