using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[ViewComponent]
	public class LMBClickViewComponent
		: AMonoViewComponent
	{
		[SerializeField]
		private LayerMask interactableMask;

		public LayerMask InteractableMask => interactableMask;

		
		[SerializeField]
		private LayerMask terrainMask;

		public LayerMask TerrainMask => terrainMask;
		
		
		[SerializeField]
		private float doubleClickTimeout = 0.3f;

		public float DoubleClickTimeout => doubleClickTimeout;

		public  float LastClickTime { get; set; }


		public bool EntityClicked { get; set; }
        
		public Entity ViewEntityClicked { get; set; }
        
        
		public bool TerrainClicked { get; set; }
        
		public Vector3 TerrainPositionClicked { get; set; }
		
		
		public EInteractionMode InteractionMode { get; set; }

		public bool Dirty { get; set; } = false;
	}
}