using System;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public struct EntitySpawnedEventComponent
	{
		public Guid GUID;

		public string PrototypeID;

		public Entity Override;
	}
}