using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.SpacePartitioning.Factories
{
	public static class SpacePartitioningFactory
	{
		public static Quadtree<TValue> BuildQuadtree<TValue>(
			Vector2 min,
			Vector2 max,
			ILoggerResolver loggerResolver = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<Quadtree<TValue>>()
				?? null;

			var result = new Quadtree<TValue>(
				RepositoriesFactory.BuildDictionaryRepository<TValue, ValueSpaceData<TValue>>(),
				NodePoolFactory.BuildNodePool<TValue>(
					loggerResolver),
				NodePoolFactory.BuildValueDataPool<TValue>(),
				new DistanceComparer2D<TValue>(),
				new Bounds2D(
					(min + max) / 2f,
					(max - min)),
				logger);

			Node<TValue> root = result.AllocateNode(
				new Bounds2D(
					min.x,
					min.y,
					max.x - min.x,
					max.y - min.y),
				null,
				0);

			result.Root = root;

			return result;
		}
	}
}