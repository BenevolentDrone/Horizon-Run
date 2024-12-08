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

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	[Visitor(typeof(Entity), typeof(EntityPrototypeDTO))]
	public class DefaultECSEntityPrototypeVisitor
		: ISaveVisitor,
          ILoadVisitor,
          IPopulateVisitor
	{
		private readonly Type[] componentTypes;

		private readonly MethodInfo writeComponentMethodInfo;

		private readonly IReadOnlyRepository<Type, WriteComponentToObjectDelegate> componentWriters;

		private readonly IEntityPrototypeRepository<TPrototypeID, TEntity> prototypeRepository;

		private readonly ILogger logger;

		public DefaultECSEntityPrototypeVisitor(
			Type[] componentTypes,
			MethodInfo writeComponentMethodInfo,
			IReadOnlyRepository<Type, WriteComponentToObjectDelegate> componentWriters,
			IEntityPrototypeRepository<TPrototypeID, TEntity> prototypeRepository,
			ILogger logger = null)
		{
			this.componentTypes = componentTypes;

			this.writeComponentMethodInfo = writeComponentMethodInfo;

			this.componentWriters = componentWriters;

			this.prototypeRepository = prototypeRepository;

			this.logger = logger;
		}

		#region IVisitor

		public bool CanVisit<TVisitable>()
		{
			return typeof(TVisitable) == typeof(Entity);
		}

		public bool CanVisit(
			Type visitableType)
		{
			return visitableType == typeof(Entity);
		}

		public Type GetDTOType<TVisitable>()
		{
			if (typeof(TVisitable) != typeof(Entity))
				return null;

			return typeof(EntityPrototypeDTO);
		}

		public Type GetDTOType(
			Type visitableType)
		{
			if (visitableType != typeof(Entity))
				return null;

			return typeof(EntityPrototypeDTO);
		}

		#endregion

		#region ISaveVisitor

		public bool VisitSave<TVisitable>(
			ref object dto,
			TVisitable visitable)
		{
			Entity entity = visitable.CastFromTo<TVisitable, Entity>();

			if (entity == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(Entity).Name}");

				dto = null;

				return false;
			}

			var entitySerializationWrapper = new EntitySerializationWrapper(entity);

			object[] componentsArray = new object[entitySerializationWrapper.Components.Length];

			for (int i = 0; i < componentsArray.Length; i++)
			{
				componentsArray[i] = entitySerializationWrapper.Components[i].ObjectValue;
			}

			string prototypeID = string.Empty;

			//TODO: optimize
			foreach (var key in prototypeRepository.AllPrototypeIDs)
			{
				if (prototypeRepository.TryGetPrototype(
					key,
					out Entity prototypeEntity))
				{
					if (prototypeEntity.Equals(entity))
					{
						prototypeID = key;

						break;
					}
				}
			}

			dto = new EntityPrototypeDTO
			{
				PrototypeID = prototypeID,

				Components = componentsArray
			};

			return true;
		}

		public bool VisitSave(
			ref object dto,
			Type visitableType,
			object visitableObject)
		{
			Entity entity = (Entity)visitableObject;

			if (entity == default)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(Entity).Name}");

				dto = null;

				return false;
			}

			var entitySerializationWrapper = new EntitySerializationWrapper(entity);

			object[] componentsArray = new object[entitySerializationWrapper.Components.Length];

			for (int i = 0; i < componentsArray.Length; i++)
			{
				componentsArray[i] = entitySerializationWrapper.Components[i].ObjectValue;
			}

			string prototypeID = string.Empty;

			//TODO: optimize
			foreach (var key in prototypeRepository.AllPrototypeIDs)
			{
				if (prototypeRepository.TryGetPrototype(
					key,
					out Entity prototypeEntity))
				{
					if (prototypeEntity.Equals(entity))
					{
						prototypeID = key;

						break;
					}
				}
			}

			dto = new EntityPrototypeDTO
			{
				PrototypeID = prototypeID,

				Components = componentsArray
			};

			return true;
		}

		#endregion

		#region ILoadVisitor

		public bool VisitLoad<TVisitable>(
			object dto,
			out TVisitable visitable)
		{
			EntityPrototypeDTO castedDTO = (EntityPrototypeDTO)dto;

			if (castedDTO.Equals(default))
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(EntityPrototypeDTO).Name}");

				visitable = default;

				return false;
			}

			prototypeRepository.TryAllocatePrototype(
				castedDTO.PrototypeID,
				out var visitableEntity);

			foreach (var component in castedDTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						visitableEntity,
						component);
			}

			visitable = visitableEntity.CastFromTo<Entity, TVisitable>();

			return true;
		}

		public bool VisitLoad(
			object dto,
			Type visitableType,
			out object visitableObject)
		{
			EntityPrototypeDTO castedDTO = (EntityPrototypeDTO)dto;

			if (castedDTO.Equals(default))
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(EntityPrototypeDTO).Name}");

				visitableObject = default;

				return false;
			}

			prototypeRepository.TryAllocatePrototype(
				castedDTO.PrototypeID,
				out var visitableEntity);

			foreach (var component in castedDTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						visitableEntity,
						component);
			}

			visitableObject = visitableEntity;

			return true;
		}

		#endregion

		#region IPopulateVisitor

		public bool VisitPopulate<TVisitable>(
			object dto,
			TVisitable visitable)
		{
			Entity entity = visitable.CastFromTo<TVisitable, Entity>();

			if (entity == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(Entity).Name}");

				return false;
			}

			EntityPrototypeDTO castedDTO = (EntityPrototypeDTO)dto;

			if (castedDTO.Equals(default))
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(EntityPrototypeDTO).Name}");

				return false;
			}

			foreach (var component in castedDTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						entity,
						component);
			}

			return true;
		}

		public bool VisitPopulate(
			object dto,
			Type visitableType,
			object visitableObject)
		{
			Entity entity = (Entity)visitableObject;

			if (entity.Equals(default))
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(Entity).Name}");

				return false;
			}

			EntityPrototypeDTO castedDTO = (EntityPrototypeDTO)dto;

			if (castedDTO.Equals(default))
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(EntityPrototypeDTO).Name}");

				return false;
			}

			foreach (var component in castedDTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						entity,
						component);
			}

			return true;
		}

		#endregion

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