using System;

using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;

using TPrototypeID = System.String;

using TEntity = DefaultEcs.Entity;

namespace HereticalSolutions.Modules.Core_DefaultECS.Factories
{
	public static class EntityPersistenceFactory
	{
		public static DefaultECSEntityPrototypeVisitor BuildDefaultECSEntityPrototypeVisitor(
			IEntityPrototypeRepository<TPrototypeID, TEntity> prototypeRepository,
			ILoggerResolver loggerResolver = null)
		{
			TypeHelpers.GetTypesWithAttributeInAllAssemblies<ComponentAttribute>(
				out Type[] componentTypes);

			MethodInfo writeComponentMethodInfo =
				typeof(DefaultECSEntityPrototypeVisitor).GetMethod(
					"WriteComponent",
					BindingFlags.Static | BindingFlags.Public);

			IReadOnlyRepository<Type, WriteComponentToObjectDelegate> componentWriters = BuildComponentWriters(
				writeComponentMethodInfo,
				componentTypes);

			return new DefaultECSEntityPrototypeVisitor(
				componentTypes,
				writeComponentMethodInfo,
				componentWriters,
				prototypeRepository,
				loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());
		}

		private static IReadOnlyRepository<Type, WriteComponentToObjectDelegate> BuildComponentWriters(
			MethodInfo writeComponentMethodInfo,
			Type[] componentTypes)
		{
			IReadOnlyRepository<Type, WriteComponentToObjectDelegate> result =
				RepositoriesFactory.BuildDictionaryRepository<Type, WriteComponentToObjectDelegate>();

			for (int i = 0; i < componentTypes.Length; i++)
			{
				MethodInfo writeComponentGeneric = writeComponentMethodInfo.MakeGenericMethod(componentTypes[i]);

				WriteComponentToObjectDelegate writeComponentGenericDelegate =
					(WriteComponentToObjectDelegate)writeComponentGeneric.CreateDelegate(
						typeof(WriteComponentToObjectDelegate),
						null);

				((IRepository<Type, WriteComponentToObjectDelegate>)result).Add(
					componentTypes[i],
					writeComponentGenericDelegate);
			}

			return result;
		}
	}
}