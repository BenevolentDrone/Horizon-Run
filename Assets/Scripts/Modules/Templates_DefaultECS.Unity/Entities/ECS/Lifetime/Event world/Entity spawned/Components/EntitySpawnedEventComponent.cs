using System;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public struct EntitySpawnedEventComponent
	{
		public Guid GUID;

		public string PrototypeID;

		public Entity Override;
	}
}