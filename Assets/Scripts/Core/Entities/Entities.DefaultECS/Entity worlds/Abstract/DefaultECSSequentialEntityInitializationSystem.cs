using System;
using System.Collections.Generic;
using System.Linq;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Entities
{
	//WHO the FUCK seals their FUCKING classes in a lib when they clearely CAN be inherited if not for behaviour modification purposes but for a simple fucking generic type specification? I hope every 'sealed' and 'internal' enjoyer gets to write a shitton of boilerplate copy-paste code for every time they cut corners with those atrocious modifiers
	public class DefaultECSSequentialEntityInitializationSystem
		: IDefaultECSEntityInitializationSystem,
		  IDisposable
	{
		private readonly ISystem<Entity>[] _systems;

		//
		// Summary:
		//     Gets or sets whether the current DefaultEcs.System.SequentialSystem`1 instance
		//     should update or not.
		public bool IsEnabled { get; set; }

		//
		// Summary:
		//     Initialises a new instance of the DefaultEcs.System.SequentialSystem`1 class.
		//
		//
		// Parameters:
		//   systems:
		//     The DefaultEcs.System.ISystem`1 instances.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     systems is null.
		public DefaultECSSequentialEntityInitializationSystem(
			IEnumerable<ISystem<Entity>> systems)
		{
			_systems = (systems ?? throw new ArgumentNullException("systems")).Where((ISystem<Entity> s) => s != null).ToArray();
			IsEnabled = true;
		}

		//
		// Summary:
		//     Initialises a new instance of the DefaultEcs.System.SequentialSystem`1 class.
		//
		//
		// Parameters:
		//   systems:
		//     The DefaultEcs.System.ISystem`1 instances.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     systems is null.
		public DefaultECSSequentialEntityInitializationSystem(params ISystem<Entity>[] systems)
			: this((IEnumerable<ISystem<Entity>>)systems)
		{
		}

		//
		// Summary:
		//     Updates all the systems once sequentially.
		//
		// Parameters:
		//   state:
		//     The state to use.
		public void Update(Entity state)
		{
			if (IsEnabled)
			{
				ISystem<Entity>[] systems = _systems;
				for (int i = 0; i < systems.Length; i++)
				{
					systems[i].Update(state);
				}
			}
		}

		//
		// Summary:
		//     Disposes all the inner DefaultEcs.System.ISystem`1 instances.
		public void Dispose()
		{
			for (int num = _systems.Length - 1; num >= 0; num--)
			{
				_systems[num].Dispose();
			}
		}
	}
}