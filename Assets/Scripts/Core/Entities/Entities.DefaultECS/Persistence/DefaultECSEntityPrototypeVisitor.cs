using System;

using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence;

using HereticalSolutions.Entities.Factories;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
	public class DefaultECSEntityPrototypeVisitor
		: ASaveLoadVisitor<Entity, EntityPrototypeDTO>
	{
		#region Reflection

		private static Type[] componentTypes;

		private static MethodInfo writeComponentMethodInfo;

		private static IReadOnlyRepository<Type, WriteComponentToObjectDelegate> componentWriters;

		#endregion

		private readonly IPrototypesRepository<World, Entity> prototypesRepository;

		public DefaultECSEntityPrototypeVisitor(
			IPrototypesRepository<World, Entity> prototypesRepository,
			ILogger logger = null)
			: base(logger)
		{
			this.prototypesRepository = prototypesRepository;
		}

		#region ILoadVisitorGeneric

		public override bool Load(
			EntityPrototypeDTO DTO,
			out Entity value)
		{
			LazyInitialization();

			prototypesRepository.TryAllocatePrototype(
				DTO.PrototypeID,
				out value);

			foreach (var component in DTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						value,
						component);
			}

			return true;
		}

		public override bool Load(
			EntityPrototypeDTO DTO,
			Entity valueToPopulate)
		{
			LazyInitialization();

			foreach (var component in DTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						valueToPopulate,
						component);
			}

			return true;
		}

		#endregion

		#region ISaveVisitorGeneric

		public override bool Save(Entity value, out EntityPrototypeDTO DTO)
		{
			var entitySerializationWrapper = new EntitySerializationWrapper(value);

			object[] componentsArray = new object[entitySerializationWrapper.Components.Length];

			for (int i = 0; i < componentsArray.Length; i++)
			{
				componentsArray[i] = entitySerializationWrapper.Components[i].ObjectValue;
			}

			string prototypeID = string.Empty;

			//TODO: optimize
			foreach (var key in prototypesRepository.AllPrototypeIDs)
			{
				if (prototypesRepository.TryGetPrototype(
					key,
					out Entity prototypeEntity))
				{
					if (prototypeEntity.Equals(value))
					{
						prototypeID = key;

						break;
					}
				}
			}

			DTO = new EntityPrototypeDTO
			{
				PrototypeID = prototypeID,

				Components = componentsArray
			};

			return true;
		}

		#endregion

		private void LazyInitialization()
		{
			if (componentTypes == null)
			{
				TypeHelpers.GetTypesWithAttribute<ComponentAttribute>(
					out componentTypes);
			}

			if (writeComponentMethodInfo == null)
			{
				writeComponentMethodInfo =
					typeof(DefaultECSEntityPrototypeVisitor).GetMethod(
						"WriteComponent",
						BindingFlags.Static | BindingFlags.Public);
			}

			if (componentWriters == null)
			{
				componentWriters = BuildComponentWriters(
					writeComponentMethodInfo,
					componentTypes);
			}
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

		public static void WriteComponent<TComponent>(
			Entity entity,
			object componentValue)
		{
			// Early return for AoT compilation calls
			if (componentValue == null)
				return;

			entity.Set<TComponent>((TComponent)componentValue);
		}
	}
}