using System;

using UnityEngine;

namespace HereticalSolutions.SpacePartitioning
{
	public interface IContainsGetPositionDelegate<TValue>
	{
		Func<TValue, Vector2> GetPositionDelegate { get; set; }
	}
}