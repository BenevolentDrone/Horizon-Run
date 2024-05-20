using System;

using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Entities.Factories;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public static class DefaultECSEntityManagedComponentWriter
	{
		private static Type[] viewComponentTypes;

		private static MethodInfo addComponentMethodInfo;

		private static IReadOnlyRepository<Type, AddObjectComponentToEntityDelegate> componentAdders;

		public static void AddViewComponentToEntity(
			Entity entity,
			object component)
		{
			LazyInitialization();

			componentAdders.Get(component.GetType()).Invoke(entity, component);
		}

		private static void LazyInitialization()
		{
			if (viewComponentTypes == null)
			{
				TypeHelpers.GetTypesWithAttribute<ViewComponentAttribute>(
					out viewComponentTypes);
			}

			if (addComponentMethodInfo == null)
			{
				addComponentMethodInfo =
					typeof(DefaultECSEntityManagedComponentWriter).GetMethod(
						"AddComponent",
						BindingFlags.Static | BindingFlags.Public);
			}

			if (componentAdders == null)
			{
				componentAdders = BuildComponentAdders(
					addComponentMethodInfo,
					viewComponentTypes);
			}
		}

		private static IReadOnlyRepository<Type, AddObjectComponentToEntityDelegate> BuildComponentAdders(
				MethodInfo addComponentMethodInfo,
				Type[] viewComponentTypes)
		{
			IReadOnlyRepository<Type, AddObjectComponentToEntityDelegate> result =
				RepositoriesFactory.BuildDictionaryRepository<Type, AddObjectComponentToEntityDelegate>();

			for (int i = 0; i < viewComponentTypes.Length; i++)
			{
				MethodInfo addComponentGeneric = addComponentMethodInfo.MakeGenericMethod(viewComponentTypes[i]);

				AddObjectComponentToEntityDelegate addComponentGenericDelegate =
					(AddObjectComponentToEntityDelegate)addComponentGeneric.CreateDelegate(
						typeof(AddObjectComponentToEntityDelegate),
						null);

				((IRepository<Type, AddObjectComponentToEntityDelegate>)result).Add(
					viewComponentTypes[i],
					addComponentGenericDelegate);
			}

			return result;
		}

		public static void AddComponent<TComponent>(
			Entity entity,
			object component)
		{
			// Early return for AoT compilation calls
			if (component == null)
				return;

			entity.Set<TComponent>((TComponent)component);
		}
	}
}