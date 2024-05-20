using System;
using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.SpacePartitioning
{
	public class DistanceComparer2D<TValue>
		: IComparer<TValue>,
		  IContainsPointOfReference<Vector2>,
		  IContainsGetPositionDelegate<TValue>
	{
		public Func<TValue, Vector2> GetPositionDelegate { get; set; }

		public Vector2 PointOfReference { get; set; }

		public int Compare(TValue a, TValue b)
		{
			return (PointOfReference - GetPositionDelegate(a)).sqrMagnitude
				.CompareTo((PointOfReference - GetPositionDelegate(b)).sqrMagnitude);
		}
	}
}