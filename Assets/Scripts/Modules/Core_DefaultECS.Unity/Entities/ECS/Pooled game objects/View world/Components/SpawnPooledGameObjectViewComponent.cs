using HereticalSolutions.Entities;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	[InitializationCommandComponent]
	[Component("View world/Initialization")]
	public struct SpawnPooledGameObjectViewComponent
	{
		public string Address;
	}
}