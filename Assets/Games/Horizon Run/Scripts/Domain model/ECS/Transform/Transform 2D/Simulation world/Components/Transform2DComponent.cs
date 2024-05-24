using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
	[Component("Simulation world/Transform")]
	public struct Transform2DComponent
	{
		public Vector2 WorldPosition;

		public float WorldRotation;

		public bool Dirty;
	}
}