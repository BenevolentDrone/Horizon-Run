using System;

using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Entities.Factories;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public static class DefaultECSEntityIDComponentGetter
	{
		private static Type[] idComponentTypes;

		private static MethodInfo tryGetComponentMethodInfo;

		private static IReadOnlyRepository<Type, TryGetIDComponentFromEntityDelegate> idComponentGetters;

		public static bool TryGetIDComponentFromEntity(
			Entity entity,
			out Type componentType,
			out object component)
		{
			LazyInitialization();

			foreach (var key in idComponentGetters.Keys)
			{
				if (idComponentGetters
					.Get(key)
					.Invoke(
						entity,
						out componentType,
						out component))
				{
					return true;
				}
			}

			componentType = null;

			component = null;

			return false;
		}

		private static void LazyInitialization()
		{
			if (idComponentTypes == null)
			{
				TypeHelpers.GetTypesWithAttribute<EntityIDComponentAttribute>(
					out idComponentTypes);
			}

			if (tryGetComponentMethodInfo == null)
			{
				tryGetComponentMethodInfo =
					typeof(DefaultECSEntityIDComponentGetter).GetMethod(
						"TryGetComponent",
						BindingFlags.Static | BindingFlags.Public);
			}

			if (idComponentGetters == null)
			{
				idComponentGetters = BuildIDComponentGetters(
					tryGetComponentMethodInfo,
					idComponentTypes);
			}
		}

		private static IReadOnlyRepository<Type, TryGetIDComponentFromEntityDelegate> BuildIDComponentGetters(
			MethodInfo addComponentMethodInfo,
			Type[] viewComponentTypes)
		{
			IReadOnlyRepository<Type, TryGetIDComponentFromEntityDelegate> result =
				RepositoriesFactory.BuildDictionaryRepository<Type, TryGetIDComponentFromEntityDelegate>();

			for (int i = 0; i < viewComponentTypes.Length; i++)
			{
				MethodInfo addComponentGeneric = addComponentMethodInfo.MakeGenericMethod(viewComponentTypes[i]);

				TryGetIDComponentFromEntityDelegate addComponentGenericDelegate =
					(TryGetIDComponentFromEntityDelegate)addComponentGeneric.CreateDelegate(
						typeof(TryGetIDComponentFromEntityDelegate),
						null);

				((IRepository<Type, TryGetIDComponentFromEntityDelegate>)result).Add(
					viewComponentTypes[i],
					addComponentGenericDelegate);
			}

			return result;
		}

		public static bool TryGetComponent<TComponent>(
			Entity entity,
			out Type componentType,
			out object component)
		{
			if (!entity.Has<TComponent>())
			{
				componentType = null;

				component = null;

				return false;
			}

			componentType = typeof(TComponent);

			component = entity.Get<TComponent>();

			return true;
		}
	}
}