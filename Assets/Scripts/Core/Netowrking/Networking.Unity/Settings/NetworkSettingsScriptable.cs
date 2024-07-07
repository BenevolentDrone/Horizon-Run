using UnityEngine;

namespace HereticalSolutions.Networking.Unity
{
    [CreateAssetMenu(fileName = "Network settings", menuName = "Settings/Network/Network settings", order = 0)]
    public class NetworkSettingsScriptable : ScriptableObject
    {
        [Header("Basic settings")]

        public NetworkBasicSettings BasicSettings;

        [Space]
        
        [Header("Default connection values")]
        
        public NetworkDefaultConnectionValuesSettings DefaultConnectionValues;

        [Space]
        
        [Header("Timeout settings")]
        
        public NetworkTimeoutSettings TimeoutSettings;
        
        [Space]
        
        [Header("Tick settings")]
        
        public NetworkTickSettings TickSettings;

        [Space]
        
        [Header("Despawn failsafe settings")]
        
        public NetworkDespawnFailsafeSettings DespawnFailsafeSettings;

        [Space]
        
        [Header("Roll call settings")]
        
        public NetworkRollCallSettings RollCallSettings;
        
        [Space]
        
        [Header("Delta replication settings")]
        
        public NetworkDeltaReplicationSettings DeltaReplicationSettings;
        
        [Space]
        
        [Header("Target FPS settings")]
        
        public NetworkTargetFPSSettings TargetFPSSettings;
        
        [Space]
        
        [Header("Position interpolation")]
        
        public NetworkPositionInterpolationSettings PositionInterpolationSettings;
        
        [Space]
        
        [Header("Rotation interpolation")]
                
        public NetworkRotationInterpolationSettings RotationInterpolationSettings;
        
        [Space]
        
        [Header("Lag simulation")]
        
        public NetworkLagSimulationSettings LagSimulationSettings;
    }
}