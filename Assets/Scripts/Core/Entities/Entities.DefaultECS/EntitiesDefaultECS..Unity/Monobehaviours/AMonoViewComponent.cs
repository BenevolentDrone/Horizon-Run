using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	/// <summary>
	/// Base class for monovehaviour view components
	/// </summary>
	public abstract class AMonoViewComponent : MonoBehaviour
	{
		public void Install(Entity viewEntity)
		{
			DefaultECSEntityManagedComponentWriter.AddViewComponentToEntity(
				viewEntity,
				this);
		}
	}
}