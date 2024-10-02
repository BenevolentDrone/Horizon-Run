using System;

namespace HereticalSolutions.Entities
{
    public interface IEntityManager<TPrototypeID, TEntityID, TEntity>
        : IReadOnlyEntityManager<TEntityID, TEntity>
    {
        #region Spawn entity

        bool SpawnEntity(
            out TEntityID entityID,
            TPrototypeID prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

        bool SpawnEntity(
            TEntityID entityID,
            TPrototypeID prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);
        
        #endregion

        #region Resolve entity

        bool ResolveEntity(
            out TEntityID entityID,
            object source,
            TPrototypeID prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

        bool ResolveEntity(
            TEntityID entityID,
            object source,
            TPrototypeID prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT);

        #endregion

        #region Despawn entity

        bool DespawnEntity(
            TEntityID entityID);

        #endregion
    }
}