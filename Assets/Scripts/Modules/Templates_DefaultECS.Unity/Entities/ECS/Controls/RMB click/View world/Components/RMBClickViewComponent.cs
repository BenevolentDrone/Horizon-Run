using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;
using UnityEngine.Serialization;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    [ViewComponent]
    public class RMBClickViewComponent
        : AMonoViewComponent
    {
        [SerializeField]
        public bool ShouldKeepIssuingMoveOrderOnMouseHold = false;
        
        [Header("Layer masks")]
        
        [SerializeField]
        private LayerMask interactableMask;

        public LayerMask InteractableMask => interactableMask;

        
        [SerializeField]
        private LayerMask terrainMask;

        public LayerMask TerrainMask => terrainMask;
        
        [Space]
        
        [Header("Click effect")]
        
        [SerializeField]
        private Transform clickEffectTransform;
        
        public Transform ClickEffectTransform => clickEffectTransform;
        
        
        [SerializeField]
        private ParticleSystem clickEffectParticleSystem;
        
        public ParticleSystem ClickEffectParticleSystem => clickEffectParticleSystem;
        

        public bool EntityClicked { get; set; }
        
        public Entity ViewEntityClicked { get; set; }


        public bool TerrainClicked { get; set; } = false;
        
        public bool TerrainClickHeld { get; set; } = false;


        public bool ClickEffectPlayed { get; set; } = false;
        

        public bool TerrainClickReleased { get; set; } = true;
        
        public Vector3 TerrainPositionClicked { get; set; }

        
        public bool Dirty { get; set; } = false;
    }
}