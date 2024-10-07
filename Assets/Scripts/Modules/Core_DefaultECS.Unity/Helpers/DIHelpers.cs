using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public static class DIHelpers
	{
		public static TDependency TryGetDependencyFromSceneContext<TDependency>()
			where TDependency : class
		{
			var contextRegistry = ProjectContext
				.Instance
				.Container
				.Resolve<SceneContextRegistry>();

			foreach (var sceneContext in contextRegistry.SceneContexts)
			{
				var result = sceneContext
					.Container
					.TryResolve<TDependency>();

				if (result != null)
					return result;
			}

			return null;
		}

		public static TDependency TryGetDependencyFromSceneContext<TDependency>(
			string id)
			where TDependency : class
		{
			var contextRegistry = ProjectContext
				.Instance
				.Container
				.Resolve<SceneContextRegistry>();

			foreach (var sceneContext in contextRegistry.SceneContexts)
			{
				var result = sceneContext
					.Container
					.TryResolveId<TDependency>(id);

				if (result != null)
					return result;
			}

			return null;
		}

		public static TDependency TryGetDependencyFromProjectContext<TDependency>()
			where TDependency : class
		{
			return ProjectContext
				.Instance
				.Container
				.TryResolve<TDependency>();
		}

		public static TDependency TryGetDependencyFromProjectContext<TDependency>(
			string id)
			where TDependency : class
		{
			return ProjectContext
				.Instance
				.Container
				.TryResolveId<TDependency>(id);
		}
	}
}