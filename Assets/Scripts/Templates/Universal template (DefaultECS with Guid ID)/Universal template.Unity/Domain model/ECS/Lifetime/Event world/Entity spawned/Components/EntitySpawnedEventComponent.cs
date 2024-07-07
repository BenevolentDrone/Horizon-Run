using System;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public struct EntitySpawnedEventComponent
	{
		public Guid GUID;

		public string PrototypeID;

		public Entity Override;
	}
}