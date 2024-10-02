using System.Collections.Generic;

using UnityEngine;
using UnityEngine.LowLevel;

namespace HereticalSolutions.Profiling
{
    public static class GlobalUnityProfiler
    {
        [RuntimeInitializeOnLoadMethod]
        private static void ApplyMarkersGlobal()
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            
            ApplyMarker(ref currentPlayerLoop);
            
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        private static void ApplyMarker(ref PlayerLoopSystem playerLoopSystem)
        {
            var subSystems = playerLoopSystem.subSystemList;
            
            if (subSystems != null)
            {
                List<PlayerLoopSystem> newSubSystems = new List<PlayerLoopSystem>();
                
                for (int i = 0; i < subSystems.Length; i++)
                {
                    var subSystem = subSystems[i];
                    
                    if (subSystem.type != default)
                    {
                        var systemName = subSystem.type.Name;

                        string markerName = $"(Unity core loop) {systemName}";

                        var marker = ProfilingManager.AllocateMarker(markerName);

                        newSubSystems.Add(
                            new PlayerLoopSystem
                            {
                                subSystemList = null,
                                updateDelegate = () =>
                                {
                                    marker.Start();
                                },
                                type = typeof(ProfilingMarker)
                            });
                        
                        ApplyMarker(ref subSystem);
                        
                        newSubSystems.Add(subSystem);
                        
                        newSubSystems.Add(
                            new PlayerLoopSystem
                            {
                                subSystemList = null,
                                updateDelegate = () =>
                                {
                                    marker.Stop();
                                },
                                type = typeof(ProfilingMarker)
                            });
                    }
                    else
                    {
                        newSubSystems.Add(subSystem);
                    }
                }
                
                playerLoopSystem.subSystemList = newSubSystems.ToArray();
            }
        }
    }
}