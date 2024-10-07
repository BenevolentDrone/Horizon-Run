namespace HereticalSolutions.Entities
{
	public interface IMultiWorldEntityManager<TWorldID, TWorld, TPrototypeID, TEntityID, TEntity>
		: IReadOnlyMultiWorldEntityManager<TWorldID, TEntityID, TEntity>,
		  IEntityManager<TPrototypeID, TEntityID, TEntity>
	{
		#region Spawn entity

		bool SpawnEntity(
			out TEntityID entityID,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

		bool SpawnEntity(
			TEntityID entityID,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

		bool SpawnWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			TWorldID worldID);

		bool SpawnWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			TEntity @override,
			TWorldID worldID);

		#endregion

		#region Resolve entity

		bool ResolveEntity(
			out TEntityID entityID,
			object source,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

		bool ResolveEntity(
			TEntityID entityID,
			object source,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

		bool ResolveWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			object source,
			TWorldID worldID);

		bool ResolveWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			object source,
			TEntity @override,
			TWorldID worldID);

		#endregion

		#region Despawn entity

		bool DespawnWorldLocalEntity(
			TEntity entity);

		#endregion
	}
}