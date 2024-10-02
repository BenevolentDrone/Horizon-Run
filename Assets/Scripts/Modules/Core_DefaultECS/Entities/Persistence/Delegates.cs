using System;
using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public delegate void WriteComponentToObjectDelegate(
		Entity entity,
		object componentValue);

	public delegate void AddObjectComponentToEntityDelegate(
		Entity entity,
		object component);

	public delegate bool TryGetIDComponentFromEntityDelegate(
		Entity entity,
		out Type componentType,
		out object component);
}