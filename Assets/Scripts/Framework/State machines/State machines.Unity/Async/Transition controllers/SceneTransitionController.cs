using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace HereticalSolutions.StateMachines
{
    public class SceneTransitionController : ITransitionController<SceneState>
    {
        public async Task EnterState(
            SceneState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
                state.SceneID,
                LoadSceneMode.Additive);
            
            while (!asyncLoad.isDone)
            {
                progress?.Report(asyncLoad.progress);
                
                await Task.Yield();
            }
            
            progress?.Report(1f);
            
            if (state.SetAsActiveOnLoad)
                SceneManager.SetActiveScene(
                    SceneManager.GetSceneByName(state.SceneID));
        }

        public async Task ExitState(
            SceneState state,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            progress?.Report(0f);
            
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(state.SceneID);
            
            while (!asyncUnload.isDone)
            {
                progress?.Report(asyncUnload.progress);
                
                await Task.Yield();
            }
            
            progress?.Report(1f);
        }
    }
}