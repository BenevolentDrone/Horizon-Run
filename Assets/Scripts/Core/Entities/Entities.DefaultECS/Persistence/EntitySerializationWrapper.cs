using System;
using System.Collections.Generic;

using DefaultEcs;
using DefaultEcs.Serialization;

namespace HereticalSolutions.Entities
{
	public class EntitySerializationWrapper
	{
		private class ComponentWrapper<T> : IComponentWrapper
		{
			public bool IsEnabled { get; }

			public T Value { get; }

			public object ObjectValue
			{
				get => Value;
			}

			public Type Type => typeof(T);

			public ComponentWrapper(bool isEnabled, T value)
			{
				Value = value;

				IsEnabled = isEnabled;
			}
		}

		private sealed class ComponentSerializationReader : IComponentReader
		{
			private readonly Entity _entity;

			private readonly List<IComponentWrapper> _components;

			public ComponentSerializationReader(in Entity entity, List<IComponentWrapper> components)
			{
				_components = components;
				_entity = entity;
			}

			public void OnRead<T>(in T component, in Entity componentOwner) => _components.Add(new ComponentWrapper<T>(_entity.IsEnabled<T>(), component));
		}

		private readonly Entity _entity;
		private readonly List<IComponentWrapper> _components;

		public World World => _entity.World;
		public bool IsAlive => _entity.IsAlive;
		public bool IsEnabled => _entity.IsEnabled();
		public IComponentWrapper[] Components => _components.ToArray();

		public EntitySerializationWrapper(Entity entity)
		{
			_entity = entity;

			_components = new List<IComponentWrapper>();

			entity.ReadAllComponents(new ComponentSerializationReader(_entity, _components));

			_components.Sort((o1, o2) => string.Compare(o1.Type.FullName, o2.Type.FullName));
		}
	}
}